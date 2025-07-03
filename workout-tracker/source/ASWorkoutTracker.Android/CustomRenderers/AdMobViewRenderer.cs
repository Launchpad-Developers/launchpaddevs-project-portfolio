using System;
using System.ComponentModel;
using Android.Gms.Ads;
using Android.Widget;
using AS.Forms.Controls.BaseControls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Content;
using ASWorkoutTracker.Droid.CustomRenderers;

[assembly: ExportRenderer(typeof(AdMobView), typeof(AdMobViewRenderer))]
namespace ASWorkoutTracker.Droid.CustomRenderers
{	
	public class AdMobViewRenderer : ViewRenderer<AdMobView, AdView>
	{
        public AdMobViewRenderer(Context context) : base(context) { }

        string adUnitId = string.Empty;
        AdSize adSize = AdSize.SmartBanner;
        AdView adView;
        AdView CreateNativeAdControl()
        {
            if (adView != null)
                return adView;

            // This is a string in the Resources/values/strings.xml 
            //EX: <string name="banner_ad_unit_id">ca-app-pub-1111111111111111~2222222222</string>
            adUnitId = Android.App.Application.Context.Resources.GetString(Resource.String.banner_ad_unit_id);
            adView = new AdView(Android.App.Application.Context);
            adView.AdSize = adSize;
            adView.AdUnitId = adUnitId;

            var adParams = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);

            adView.LayoutParameters = adParams;

            adView.LoadAd(new AdRequest
                            .Builder()
                            .Build());
            return adView;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<AdMobView> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
            {
                CreateNativeAdControl();
                SetNativeControl(adView);
            }
        }
        //public AdMobViewRenderer(Context context) : base(context) { }

        //protected override void OnElementChanged(ElementChangedEventArgs<AdMobView> e)
        //{
        //	base.OnElementChanged(e);

        //	if (e.NewElement != null && Control == null)
        //		SetNativeControl(CreateAdView());
        //}

        //protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //	base.OnElementPropertyChanged(sender, e);

        //	if (e.PropertyName == nameof(AdView.AdUnitId))
        //		Control.AdUnitId = Element.AdUnitId;
        //}

        //private AdView CreateAdView()
        //{
        //	var adView = new AdView(Context)
        //	{
        //		AdSize = AdSize.SmartBanner,
        //		AdUnitId = Element.AdUnitId
        //	};

        //	adView.LayoutParameters = new LinearLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);

        //	adView.LoadAd(new AdRequest.Builder().Build());

        //	return adView;
        //}
    }
}
//using Android.Widget;
//using Android.Gms.Ads;
//using Xamarin.Forms;
//using Xamarin.Forms.Platform.Android;

//[assembly: ExportRenderer(typeof(MeetupManager.Controls.AdControlView), typeof(MeetupManager.Droid.PlatformSpecific.AdViewRenderer))]
//namespace MeetupManager.Droid.PlatformSpecific
//{
//    public class AdViewRenderer : ViewRenderer<Controls.AdControlView, AdView>
//    {
//        string adUnitId = string.Empty;
//        //Note you may want to adjust this, see further down.
//        AdSize adSize = AdSize.SmartBanner;
//        AdView adView;
//        AdView CreateNativeAdControl()
//        {
//            if (adView != null)
//                return adView;

//            // This is a string in the Resources/values/strings.xml that I added or you can modify it here. This comes from admob and contains a / in it
//            adUnitId = Forms.Context.Resources.GetString(Resource.String.banner_ad_unit_id);
//            adView = new AdView(Forms.Context);
//            adView.AdSize = adSize;
//            adView.AdUnitId = adUnitId;

//            var adParams = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);

//            adView.LayoutParameters = adParams;

//            adView.LoadAd(new AdRequest
//                            .Builder()
//                            .Build());
//            return adView;
//        }

//        protected override void OnElementChanged(ElementChangedEventArgs<Controls.AdControlView> e)
//        {
//            base.OnElementChanged(e);
//            if (Control == null)
//            {
//                CreateNativeAdControl();
//                SetNativeControl(adView);
//            }
//        }
//    }
//}