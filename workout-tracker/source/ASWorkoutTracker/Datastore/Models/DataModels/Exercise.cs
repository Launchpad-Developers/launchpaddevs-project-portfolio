using System;
using System.Collections.Generic;
using ASCommonServices.Models;
using SQLite;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ASWorkoutTracker.Datastore.Models
{
    public class Exercise : BaseSQLModel, IEquatable<BaseSQLModel>
    {
        public Exercise()
        {
            LastSets = new List<ExerciseLog>();
        }

        [NotNull]
        public int BodyAreaTypeID { get; set; }
        [NotNull]
        public int ExerciseTypeID { get; set; }
        [NotNull]
        public int EquipmentTypeID { get; set; }
        [NotNull]
        public int LevelID { get; set; }
        [NotNull]
        public bool Selected { get; set; }
        [MaxLength(5000)]
        public string Description { get; set; }
        [MaxLength(500)]
        public string Notes { get; set; }

        public string Url { get; set; }
        public string Citation { get; set; }
        public bool IsFavorite { get; set; }

        private bool _isShowingDetails;
        [Ignore]
        public bool IsShowingDetails
        {
            get { return _isShowingDetails; }
            set { _isShowingDetails = value; OnPropertyChanged(nameof(IsShowingDetails)); }
        }

        [Ignore]
        public BodyAreaType BodyAreaType { get; set; }
        [Ignore]
        public ExerciseType ExerciseType { get; set; }
        [Ignore]
        public EquipmentType EquipmentType { get; set; }
        [Ignore]
        public Level Level { get; set; }
        [Ignore]
        public List<ExerciseLog> LastSets { get; set; }
        [Ignore]
        public DateTime LastSetDate { get; set; }
        [Ignore]
        public bool IsNew { get { return LastSetDate == null || LastSetDate == DateTime.MinValue; } }
        [Ignore]
        public string LastSetDetail { get; set; }
        [Ignore]
        public string TotalSets { get; set; }
        [Ignore]
        public string TotalDetail { get; set; }

        [Ignore]
        private string _citationHtml
        {
            get
            {
                var citeSource = !string.IsNullOrEmpty(Citation) ? Citation : Url;
                var cite = $"<a href='{citeSource}'>{citeSource}</a>";

                return $"<p><span style='font-size: 0.5em;'><strong>Source:</strong> <cite>{cite}</cite></span></p>";
            }
        }

        [Ignore]
        private string _platformFont { get { return DeviceInfo.Platform == DevicePlatform.iOS ? "font-family:KohinoorBangla-Regular;" : "font-family:Montserrat-Light;"; } }
        [Ignore]
        private string _bodyFontSize { get { return DeviceInfo.Platform == DevicePlatform.iOS ? "font-size: 3em;" : "font-size: 1.5em;"; } }

        [Ignore]
        public HtmlWebViewSource WebViewHighlights { get { return new HtmlWebViewSource { Html = $@"<html><body style='{_bodyFontSize}{_platformFont}'>{Notes}<br />{BodyAreaType.WebViewDetail}{Level.WebViewDetail}{EquipmentType.WebViewDetail}{_citationHtml}</body></html>" }; } }
        [Ignore]
        public HtmlWebViewSource WebViewNotes { get { return new HtmlWebViewSource { Html = $@"<html><body style='{_bodyFontSize}{_platformFont}'>{Notes}{_citationHtml}</body></html>" }; } }
        [Ignore]
        public HtmlWebViewSource WebViewDescription { get { return new HtmlWebViewSource { Html = $@"<html><body style='{_bodyFontSize}{_platformFont}'>{Description}{_citationHtml}</body></html>" }; } }
               
        public override bool Equals(object obj)
        {
            var other = (Exercise)obj;
//#if DEBUG
//            Console.WriteLine($"{Name} Compare THIS vs OTHER");
//            Console.WriteLine($"Name:....................{Name} : {other.Name}");
//            Console.WriteLine($"BodyAreaTypeID:..........{BodyAreaTypeID} : {other.BodyAreaTypeID}");
//            Console.WriteLine($"ExerciseTypeID:..........{ExerciseTypeID} : {other.ExerciseTypeID}");
//            Console.WriteLine($"EquipmentTypeID:.........{EquipmentTypeID} : {other.EquipmentTypeID}");
//            Console.WriteLine($"Description:.............{Description} : {other.Description}");
//            Console.WriteLine($"Notes:...................{Notes} : {other.Notes}");
//            Console.WriteLine($"Url:.....................{Url} : {other.Url}");
//#endif
            return other.Name == Name &&
                   other.BodyAreaTypeID == BodyAreaTypeID &&
                   other.ExerciseTypeID == ExerciseTypeID &&
                   other.EquipmentTypeID == EquipmentTypeID &&
                   other.LevelID == LevelID &&
                   other.Description == Description &&
                   other.Notes == Notes &&
                   other.Url == Url;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
