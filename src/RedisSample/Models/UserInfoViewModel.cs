using System.ComponentModel.DataAnnotations;

namespace RedisSample.Models
{
    public class UserInfoViewModel
    {
        public bool EditAnswers { get; set; }

        [Display(Name = "Your favorite color:")]
        public string FavoriteColor { get; set; }

        [Display(Name = "Name of your favorite pet:")]
        public string FavoritePet { get; set; }

        [Display(Name = "Server instance:")]
        public string Instance { get; set; }

        [Display(Name = "Your name:")]
        public string UserName { get; set; }

        public bool SurveyFinished
        {
            get
            {
                return (!string.IsNullOrEmpty(FavoriteColor) && !string.IsNullOrEmpty(FavoritePet) &&
                    !string.IsNullOrEmpty(UserName));
            }
        }
    }
}
