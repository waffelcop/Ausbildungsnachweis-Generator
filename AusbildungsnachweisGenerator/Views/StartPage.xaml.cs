﻿using AusbildungsnachweisGenerator.Extensions;
using AusbildungsnachweisGenerator.Model;
using AusbildungsnachweisGenerator.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using AusbildungsnachweisGenerator;
using AusbildungsnachweisGenerator.Helper;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AusbildungsnachweisGenerator.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StartPage : Page
    {
        private double progress = double.NaN;
        public double Progress
        {
            get => progress;
            set
            {
                progress = value;
                if (!double.IsNaN(progress))
                {
                    GenerateButton.IsEnabled = false;
                    if (progress > 2)
                    {
                        ProgressRingMain.IsActive = true;
                        ProgressRingMain.IsIndeterminate = false;
                        ProgressRingMain.Value = progress;
                    }
                    else
                    {
                        ProgressRingMain.IsActive = true;
                        ProgressRingMain.IsIndeterminate = true;
                        ProgressRingMain.Value = 0;
                    }
                }
                else
                {
                    ProgressRingMain.IsActive = false;
                    ProgressRingMain.IsIndeterminate = true;
                    ProgressRingMain.Value = 0;
                    SetGenerateButtonIsEnabledBinding();
                }
            }
        }

        public StartPage()
        {
            this.InitializeComponent();
            this.DataContext = new StartPageViewModel();
            SetGenerateButtonIsEnabledBinding();
        }
        private void SetGenerateButtonIsEnabledBinding()
        {
            var binding = new Binding();
            binding.Path = new PropertyPath("IsFormValid");
            binding.Mode = BindingMode.OneWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding(GenerateButton, Button.IsEnabledProperty, binding);
        }
        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            Progress = 1;
            var dt = (StartPageViewModel)DataContext;
            new Task(() => GenerateProofs(dt)).Start();
        }

        private void GenerateProofs(StartPageViewModel vm)
        {
            try
            {
                StartPageViewModel dataContext = vm;

                var generated = 0;
                var max = 0;

                var profile = dataContext.SelectedProfile;
                var noteNr = 0;

                var startDate = dataContext.StartDate.DateTime.StartOfWeek();
                var endDate = dataContext.EndDate.DateTime.EndOfWeek();

                var years = startDate.GetYearlyDateRangeTo(endDate);
                foreach (var year in years)
                {
                    var yearPath = @$"{dataContext.FilePath}\{year.Year}";
                    Directory.CreateDirectory(yearPath);

                    noteNr++;

                    var months = startDate.GetMonthlyDateRangeTo(endDate);
                    foreach (var month in months)
                    {
                        if (month.Year == year.Year)
                        {
                            var monthPath = @$"{yearPath}\{month.Month} {month.ToString("MMMM")}";
                            Directory.CreateDirectory(monthPath);

                            var weeks = startDate.GetWeeklyDateRangeTo(endDate);

                            if (max == 0)
                                max = weeks.Count();

                            foreach (var week in weeks)
                            {
                                if (week.Month == month.Month && week.Year == month.Year)
                                {
                                    var proof = new Proof(noteNr.ToString(),
                                        profile.Apprenticeship,
                                        profile.Apprentice,
                                        profile.Address,
                                        profile.Instructor,
                                        profile.Job,
                                        profile.Company,
                                        noteNr,
                                        week.StartOfWeek(),
                                        week.EndOfWeek(),
                                        hourRate: profile.Apprenticeship.HourRate);
                                    proof.GenerateDocument(monthPath);
                                    generated++;

                                    DispatcherQueue.TryEnqueue(() =>
                                    {
                                        Progress = ((double)generated / (double)max)*100;
                                    });
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            DispatcherQueue.TryEnqueue(() =>
            {
                Progress = double.NaN;
            });
        }

        private async void FilePathButton_Click(object sender, RoutedEventArgs e)
        {
            var dataContext = (StartPageViewModel)DataContext;

            dataContext.FilePath = await IOHelper.SelectFolder();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ((StartPageViewModel)DataContext).LoadProfiles();
        }
    }
}
