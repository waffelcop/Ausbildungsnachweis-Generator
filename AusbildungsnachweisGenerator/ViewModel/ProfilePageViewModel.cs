﻿using AusbildungsnachweisGenerator.Model;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AusbildungsnachweisGenerator.ViewModel
{
    internal class ProfilePageViewModel : ObservableObject
    {
        private Job job;
        private Instructor instructor;
        private Company company;
        private Apprenticeship apprenticeship;
        private Apprentice apprentice;
        private Address address;
        private Profile selectedProfile;
        private List<Profile> profiles;
        private int selectedProfileIndex;

        public static Proof Sample { get; } = Proof.Sample;

        public Apprentice Apprentice
        {
            get => apprentice;
            set
            {
                SetProperty(ref apprentice, value);
                OnPropertyChanged(nameof(IsFormValid));
            }
        }
        public Address Address
        {
            get => address;
            set => SetProperty(ref address, value);
        }
        public Apprenticeship Apprenticeship
        {
            get => apprenticeship;
            set => SetProperty(ref apprenticeship, value);
        }
        public Company Company
        {
            get => company;
            set
            {
                SetProperty(ref company, value);
                OnPropertyChanged(nameof(IsFormValid));
            }
        }
        public Instructor Instructor
        {
            get => instructor;
            set => SetProperty(ref instructor, value);
        }
        public Job Job
        {
            get => job;
            set => SetProperty(ref job, value);
        }

        public List<Profile> Profiles
        {
            get => profiles;
            set
            {
                SetProperty(ref profiles, value);
            }
        }

        public Profile SelectedProfile
        {
            get => selectedProfile;
            set
            {
                SetProperty(ref selectedProfile, value);
                SetData(selectedProfile);
                OnPropertyChanged(nameof(IsFormValid));
                OnPropertyChanged(nameof(CanBeDeleted));
            }
        }
        public bool IsFormValid =>  SelectedProfile != null &&
                                    !string.IsNullOrWhiteSpace(Apprentice?.Firstname) &&
                                    !string.IsNullOrWhiteSpace(Apprentice?.Name) &&
                                    !string.IsNullOrWhiteSpace(Company?.Name);
        public bool CanBeDeleted =>  SelectedProfile != null && !SelectedProfile.IsForCreation;
        public ProfilePageViewModel()
        {
        }

        internal void RefreshFormValidation()
        {
            OnPropertyChanged(nameof(IsFormValid));
        }

        private void SetData(Profile profile)
        {
            Job = profile?.Job ?? new();
            Instructor = profile?.Instructor ?? new();
            Company = profile?.Company ?? new();
            Apprenticeship = profile?.Apprenticeship ?? new();
            Apprentice = profile?.Apprentice ?? new();
            Address = profile?.Address ?? new();
        }
        public void LoadProfiles()
        {
            var profiles = AppHelper.GetSettings().Profiles;
            var emptyProfile = Profile.New;
            profiles.Add(emptyProfile);
            Profiles = profiles;
            SelectedProfile = Profiles.Last();
        }
    }
}
