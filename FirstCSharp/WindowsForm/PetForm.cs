﻿using System.Dynamic;
using System.Globalization;
using VetmanagerApiGateway;
using VetmanagerApiGateway.DTO.ModelContainer;
using VetmanagerApiGateway.DTO.ModelContainer.Model;
using VetmanagerApiGateway.DTO.ModelContainer.Model.Enum;
using VetmanagerApiGateway.PathUri;

namespace VetmanagerFormControl.WindowsForm
{
    internal partial class PetForm : Form
    {
        private readonly UserList _userList;
        private readonly ApiGateway _vetmanagerApiGateway;
        private readonly Pet? _petToEdit;
        private readonly PetType[] _petTypesWithBreeds;
        private readonly int _ownerId;

        public PetForm(UserList userList, ApiGateway vetmanagerApiGateway, int ownerId, PetType[] petTypes) : this(userList, vetmanagerApiGateway, ownerId, petTypes, null)
        {
        }

        public PetForm(UserList userList, ApiGateway vetmanagerApiGateway, int ownerId, PetType[] petTypesWithBreeds, Pet? petToEdit)
        {
            InitializeComponent();
            _userList = userList;
            _vetmanagerApiGateway = vetmanagerApiGateway;
            _ownerId = ownerId;
            _petToEdit = petToEdit;
            _petTypesWithBreeds = petTypesWithBreeds;
            typeComboBox.DataSource = _petTypesWithBreeds;
            typeComboBox.DisplayMember = "Title";
            typeComboBox.ValueMember = "Id";
            typeComboBox.SelectedItem = null;
            breedComboBox.DisplayMember = "Title";
            breedComboBox.ValueMember = "Id";
            breedComboBox.SelectedItem = null;
            genderComboBox.DataSource = Enum.GetValues(typeof(PetGender));
            FillPetDataIfGiven();
        }

        private void FillPetDataIfGiven()
        {
            if (_petToEdit == null) { return; }

            saveButton.Text = "Update Form";
            this.Text = "Update Pet";
            aliasTextBox.Text = _petToEdit.alias;

            if (_petToEdit.type_id != null)
            {
                typeComboBox.SelectedItem = GetPetTypeByIdFromList(Int32.Parse(_petToEdit.type_id));
                FillBreedsIntoComboxUsingSelectedPetType();

                if (_petToEdit.breed_id != null)
                {
                    breedComboBox.SelectedItem = GetBreedByIdFromList(Int32.Parse(_petToEdit.breed_id));
                }
                else { breedComboBox.SelectedItem = null; }
            }

            if (_petToEdit.sex != null) { genderComboBox.SelectedItem = (PetGender)Enum.Parse(typeof(PetGender), _petToEdit.sex); }
            if (_petToEdit.birthday != null) { birthdayDateTimePicker.Value = DateTime.ParseExact(_petToEdit.birthday, "yyyy-MM-dd", new CultureInfo("ru-RU")); }
        }

        private PetType GetPetTypeByIdFromList(int petTypeId)
        {
            foreach (PetType petType in _petTypesWithBreeds)
            {
                if (petType.Id == petTypeId) { return petType; }
            }
            throw new Exception("Failed to find PetType by Id in list");
        }

        private Breed GetBreedByIdFromList(int breedId)
        {
            Breed[] breeds = GetBreedsForSelectedPetType();
            foreach (Breed breed in breeds)
            {
                if (breed.Id == breedId) { return breed; }
            }
            throw new Exception("Failed to find Breed by Id in list");
        }

        private async void saveButton_Click(object sender, EventArgs e)
        {
            dynamic petObjectToSend = new ExpandoObject();
            petObjectToSend.owner_id = _ownerId;
            petObjectToSend.alias = aliasTextBox.Text;
            SetPetTypeIdIfSelected(petObjectToSend);
            SetBreedIdIfSelected(petObjectToSend);
            petObjectToSend.sex = genderComboBox.Text;
            petObjectToSend.birthday = birthdayDateTimePicker.Value.ToString("yyyy-MM-dd");

            try
            {
                if (_petToEdit == null) { PetDataAfterPostRequest petRootData = await _vetmanagerApiGateway.CreateModel<PetDataAfterPostRequest>(new PathUri(AccessibleModelPathUri.pet), petObjectToSend); }
                else { PetData petRootData = await _vetmanagerApiGateway.UpdateModel<PetData>(new PathUri(AccessibleModelPathUri.pet, _petToEdit.Id), petObjectToSend); }

                _userList.UpdatePetTable();
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show("Exception message: " + ex.Message); }
        }

        private void typeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillBreedsIntoComboxUsingSelectedPetType();
        }

        private void FillBreedsIntoComboxUsingSelectedPetType()
        {
            breedComboBox.DataSource = GetBreedsForSelectedPetType();
        }

        private Breed[] GetBreedsForSelectedPetType()
        {
            int? selectedPetTypeIdNullable = GetSelectedPetTypeIdNullable();

            if (selectedPetTypeIdNullable is null)
            {
                return Array.Empty<Breed>();
            }

            int selectedPetTypeId = GetSelectedPetTypeIdOrThrow();

            foreach (PetType petType in _petTypesWithBreeds)
            {
                if (petType.Id == selectedPetTypeId)
                {
                    return petType.Breeds ?? Array.Empty<Breed>();
                }
            }

            return Array.Empty<Breed>();
        }

        private void SetPetTypeIdIfSelected(dynamic pet)
        {
            int? selectedPetTypeId = GetSelectedPetTypeIdNullable();
            if (selectedPetTypeId is not null)
            {
                pet.type_id = selectedPetTypeId;
            }
        }

        private int GetSelectedPetTypeIdOrThrow()
        {
            int? selectedPetTypeIdNullable = GetSelectedPetTypeIdNullable();
            return selectedPetTypeIdNullable ?? throw new Exception("Somehow Pet Type Id was null");
        }

        private int? GetSelectedPetTypeIdNullable()
        {
            string? selectedPetTypeId = typeComboBox.GetItemText(typeComboBox.SelectedValue);
            return (String.IsNullOrEmpty(selectedPetTypeId)) ? null : Int32.Parse(selectedPetTypeId);
        }

        private void SetBreedIdIfSelected(dynamic pet)
        {
            int? selectedBreedId = GetSelectedBreedIdNullable();
            if (selectedBreedId is not null)
            {
                pet.breed_id = selectedBreedId;
            }
        }

        private int? GetSelectedBreedIdNullable()
        {
            string? selectedBreedId = breedComboBox.GetItemText(breedComboBox.SelectedValue);
            return (String.IsNullOrEmpty(selectedBreedId)) ? null : Int32.Parse(selectedBreedId);
        }
    }
}
