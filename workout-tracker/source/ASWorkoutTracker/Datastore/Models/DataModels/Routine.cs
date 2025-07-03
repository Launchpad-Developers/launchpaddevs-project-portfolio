using System;
using ASCommonServices.Models;
using SQLite;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ASWorkoutTracker.Datastore.Models
{
    public class Routine : BaseSQLModel
    {

        public int UserID { get; set; }
        public int ProgramID { get; set; }
        public string Url { get; set; }
        public string Citation { get; set; }
        public bool IsFavorite { get; set; }
        [MaxLength(5000)]
        public string Description { get; set; }
        [MaxLength(500)]
        public string Notes { get; set; }

        /// <summary>
        /// Each Routine can be based on an Exercise Type
        /// (Strength, Endurance, Flexibility, Balance)
        /// </summary>
        //[NotNull]
        //public int RoutineTypeID { get; set; }
        
        [Ignore]
        public bool IsNew { get { return DateLastUpdated == null || DateLastUpdated == DateTime.MinValue; } }
        //[Ignore]
        //public RoutineType RoutineType { get; set; }
        [Ignore]
        public Program Program { get; set; }

        private bool _isShowingDetails;
        [Ignore]
        public bool IsShowingDetails
        {
            get { return _isShowingDetails; }
            set { _isShowingDetails = value; OnPropertyChanged(nameof(IsShowingDetails)); }
        }

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
        public HtmlWebViewSource WebViewNotes { get { return new HtmlWebViewSource { Html = $@"<html><body style='{_bodyFontSize}{_platformFont}'>{Notes}{_citationHtml}</body></html>" }; } }
        [Ignore]
        public HtmlWebViewSource WebViewDescription { get { return new HtmlWebViewSource { Html = $@"<html><body style='{_bodyFontSize}{_platformFont}'>{Description}{_citationHtml}</body></html>" }; } }

        public override bool Equals(object obj)
        {
            var other = (Routine)obj;
//#if DEBUG
//            Console.WriteLine($"{Name} Compare THIS vs OTHER");
//            Console.WriteLine($"Name:....................{Name} : {other.Name}");
//            Console.WriteLine($"Description:.............{Description} : {other.Description}");
//            Console.WriteLine($"Notes:...................{Notes} : {other.Notes}");
//#endif
            return other.Name == Name &&
                   other.Description == Description &&
                   other.Notes == Notes;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
