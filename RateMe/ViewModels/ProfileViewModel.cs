using RateMe.Models.ClientModels;
using RateMe.Models.JsonFileModels;
using RateMe.Services;

namespace RateMe.ViewModels;

public class ProfileViewModel
{
    public UserService UserService { get; }
    
    public DataHintTextModel NameModel { get; }
    public DataHintTextModel SurnameModel { get; }
    public DataHintTextModel CurriculumModel { get; }
    public DataHintTextModel EmailModel { get; }
    public DataHintTextModel AboutModel { get; }

    public ProfileViewModel(UserService userService)
    {
        UserService = userService;
        NameModel = new DataHintTextModel(UserService.User!.Name, "Имя");
        SurnameModel = new DataHintTextModel(UserService.User!.Surname, "Фамилия");
        CurriculumModel = new DataHintTextModel("SE 2028", "оп");
        EmailModel = new DataHintTextModel(UserService.User!.Email, "Email");
        AboutModel = new DataHintTextModel("You know after all that...", "О себе");
    }

    public ProfileViewModel() {}

    public void SaveChanges()
    {
        UserService.User!.Name = NameModel.Data;   // TODO: profile picture + push in db + check for actual changes
        UserService.User!.Surname = SurnameModel.Data;
        UserService.User!.Email = EmailModel.Data;  // Rn just change, maybe after add validations
        JsonFileModelsHelper.SaveUser(UserService.User!);
    }
}