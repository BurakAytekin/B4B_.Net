using System.Web.Optimization;

namespace B2b.Web.v4
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region B2B Bundles

            bundles.Add(new StyleBundle("~/bundles/fancyboxCss").Include(
                "~/Content/fancybox/css/jquery.fancybox.css"
            ));

            bundles.Add(new StyleBundle("~/bundles/iztoastcss").Include(
                "~/Content/izToast/css/iziToast.css"
            ));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                "~/Content/css/bootstrap.min.css",
                "~/Content/css/font-awesome.min.css",
                "~/Content/css/prettyPhoto.css",
                "~/Content/css/revslider.css",
                "~/Content/css/sumoselect.css",
                "~/Content/css/owl.carousel.css",
                "~/Content/css/style.css",
                "~/Content/izToast/css/iziToast.css",
                "~/Content/css/responsive.css",
                "~/Content/jquery-confirm/css/jquery-confirm.css",
                "~/Content/tooltipster/css/tooltipster.bundle.css",
                "~/Content/bootstrap-slider/css/bootstrap-slider.css",
                     "~/Scripts/Admin/vendor/datetimepicker/css/bootstrap-datetimepicker.min.css",
                "~/Content/fancybox/css/jquery.fancybox.css",
                "~/Content/ng-table.css",
                "~/Content/css/flag-icon.css"
            ));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/Js/jquery-{version}.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/Js/modernizr.custom.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/Js/bootstrap.min.js",
                "~/Scripts/Js/smoothscroll.js",
                "~/Scripts/Js/jquery.debouncedresize.js",
                "~/Scripts/Js/retina.min.js",
                "~/Scripts/Js/jquery.placeholder.js",
                "~/Scripts/Js/jquery.hoverIntent.min.js",
                "~/Scripts/Js/twitter/jquery.tweet.min.js",
                "~/Scripts/Js/jquery.flexslider-min.js",
                "~/Scripts/Js/owl.carousel.min.js",
                "~/Scripts/Js/jflickrfeed.min.js",
                "~/Scripts/Js/jquery.sumoselect.js",
                "~/Scripts/Js/jquery.prettyPhoto.js",
                "~/Scripts/Js/jquery.themepunch.tools.min.js",
                "~/Scripts/Js/jquery.themepunch.revolution.min.js",
                "~/Scripts/Js/shortcut.js",
                "~/Content/izToast/js/iziToast.js",
                "~/Content/jquery-confirm/js/jquery-confirm.js",
                "~/Content/tooltipster/js/tooltipster.bundle.js",
                "~/Content/fancybox/js/jquery.fancybox.js",
                "~/Content/bootstrap-slider/js/bootstrap-slider.js",
                "~/Scripts/jquery.number.js",
                    "~/Scripts/Admin/vendor/daterangepicker/moment.min.js",
                      "~/Scripts/Admin/vendor/moment/locales.min.js",
                  "~/Scripts/Admin/vendor/datetimepicker/js/bootstrap-datetimepicker.min.js",
                "~/Scripts/Js/main.js",
                "~/Scripts/less.min.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                "~/Scripts/angular.js",
                "~/Scripts/angular-mocks.js",
                "~/Scripts/angular-sanitize.js",
                "~/Scripts/ng-table.js",
                "~/Scripts/angular-resource.min.js", "~/Scripts/Pages/angular-base64-upload.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/fancyboxjs").Include(
                "~/Content/fancybox/js/jquery.fancybox.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/iztoastjs").Include(
                "~/Content/izToast/js/iziToast.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/paymentjs").Include(
                "~/Scripts/Payment/card.js",
                "~/Scripts/Payment/jquery.smartWizard.js",
                "~/Scripts/Payment/customPayment.js"
            ));

            #endregion

            #region Admin Bundles

            bundles.Add(new StyleBundle("~/bundles/Admin/cssPlugins").Include(
                "~/Content/Admin/vendor/bootstrap.css",
                "~/Content/Admin/vendor/animate.css",
                "~/Content/Admin/vendor/font-awesome.min.css",
                "~/Content/Admin/vendor/font-awesome-animation.min.css",
                "~/Scripts/Admin/vendor/animsition/css/animsition.min.css",
                "~/Scripts/Admin/vendor/daterangepicker/daterangepicker-bs3.css",
                "~/Scripts/Admin/vendor/morris/morris.css",
                "~/Scripts/Admin/vendor/owl-carousel/owl.carousel.css",
                "~/Scripts/Admin/vendor/owl-carousel/owl.theme.css",
                "~/Scripts/Admin/vendor/rickshaw/rickshaw.min.css",
                "~/Scripts/Admin/vendor/datetimepicker/css/bootstrap-datetimepicker.min.css",
                "~/Scripts/Admin/vendor/datatables/css/jquery.dataTables.min.css",
                "~/Scripts/Admin/vendor/datatables/datatables.bootstrap.min.css",
                "~/Scripts/Admin/vendor/chosen/chosen.css",
                "~/Scripts/Admin/vendor/summernote/summernote.css",
                "~/Content/jquery-confirm/css/jquery-confirm.css",
                "~/Content/fancybox/css/jquery.fancybox.css",
                "~/Content/izToast/css/iziToast.css",
                "~/Content/ng-table.css"
            ));

            bundles.Add(new StyleBundle("~/bundles/Admin/cssMain").Include(
                "~/Content/Admin/main.css"
            ));

            bundles.Add(new ScriptBundle("~/bundles/Admin/jsModernizr").Include(
                "~/Scripts/Admin/vendor/modernizr/modernizr-2.8.3-respond-1.4.2.min.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/Admin/jQuery").Include(
                "~/Scripts/Js/jquery-{version}.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/Admin/jsBootstrap").Include(
                "~/Scripts/Admin/vendor/bootstrap/bootstrap.min.js",
                "~/Scripts/Admin/vendor/jRespond/jRespond.min.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/Admin/jsPlugins").Include(
                "~/Scripts/Admin/vendor/d3/d3.min.js",
                "~/Scripts/Admin/vendor/d3/d3.layout.min.js",
                "~/Scripts/Admin/vendor/rickshaw/rickshaw.min.js",
                "~/Scripts/Admin/vendor/sparkline/jquery.sparkline.min.js",
                "~/Scripts/Admin/vendor/slimscroll/jquery.slimscroll.min.js",
                "~/Scripts/Admin/vendor/animsition/js/jquery.animsition.min.js",
                "~/Scripts/Admin/vendor/daterangepicker/moment.min.js",
                "~/Scripts/Admin/vendor/moment/locales.min.js",
                "~/Scripts/Admin/vendor/daterangepicker/daterangepicker.js",
                "~/Scripts/Admin/vendor/screenfull/screenfull.min.js",
                "~/Scripts/Admin/vendor/flot/jquery.flot.min.js",
                "~/Scripts/Admin/vendor/flot-tooltip/jquery.flot.tooltip.min.js",
                "~/Scripts/Admin/vendor/flot-spline/jquery.flot.spline.min.js",
                "~/Scripts/Admin/vendor/easypiechart/jquery.easypiechart.min.js",
                "~/Scripts/Admin/vendor/raphael/raphael-min.js",
                "~/Scripts/Admin/vendor/morris/morris.min.js",
                "~/Scripts/Admin/vendor/owl-carousel/owl.carousel.min.js",
                "~/Scripts/Admin/vendor/datetimepicker/js/bootstrap-datetimepicker.min.js",
                "~/Scripts/Admin/vendor/datatables/js/jquery.dataTables.min.js",
                "~/Scripts/Admin/vendor/datatables/extensions/dataTables.bootstrap.js",
                "~/Scripts/Admin/vendor/chosen/chosen.jquery.min.js",
                "~/Scripts/Admin/vendor/summernote/summernote.min.js",
                "~/Scripts/Admin/vendor/coolclock/coolclock.js",
                "~/Scripts/Admin/vendor/coolclock/excanvas.js",
                "~/Content/fancybox/js/jquery.fancybox.js",
                "~/Content/izToast/js/iziToast.js",
                "~/Content/jquery-confirm/js/jquery-confirm.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/Admin/jsMain").Include(
                "~/Scripts/jquery.number.js",
                "~/Scripts/session_timeout.min.js",
                "~/Scripts/Admin/main.js"
            ));

            #endregion

            #region Admin Blank Layout Bundles

            bundles.Add(new StyleBundle("~/bundles/Admin/cssBlankPlugins").Include(
                "~/Content/Admin/vendor/bootstrap.min.css",
                "~/Content/Admin/vendor/animate.css",
                "~/Content/Admin/vendor/font-awesome.min.css",
                "~/Scripts/Admin/vendor/animsition/css/animsition.min.css"
            ));

            bundles.Add(new StyleBundle("~/bundles/Admin/cssBlankMain").Include(
                "~/Content/Admin/main.css"
            ));

            bundles.Add(new ScriptBundle("~/bundles/Admin/jsBlankModernizr").Include(
                "~/Scripts/Admin/vendor/modernizr/modernizr-2.8.3-respond-1.4.2.min.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/Admin/jQueryBlank").Include(
                "~/Scripts/Js/jquery-{version}.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/Admin/jsBlankBootstrap").Include(
                "~/Scripts/Admin/vendor/bootstrap/bootstrap.min.js",
                "~/Scripts/Admin/vendor/jRespond/jRespond.min.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/Admin/jsBlankPlugins").Include(
                "~/Scripts/Admin/vendor/sparkline/jquery.sparkline.min.js",
                "~/Scripts/Admin/vendor/slimscroll/jquery.slimscroll.min.js",
                "~/Scripts/Admin/vendor/animsition/js/jquery.animsition.min.js",
                "~/Scripts/Admin/vendor/screenfull/screenfull.min.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/Admin/jsBlankMain").Include(
                "~/Scripts/jquery.number.js",
                "~/Scripts/Admin/main.js"
            ));

            #endregion

            #region General
            bundles.Add(new ScriptBundle("~/bundles/jsAdminApp").Include(
                "~/Scripts/angularApp.js"
            ));
            #endregion

            #region Custom DataTable(KendoUI)
            bundles.Add(new ScriptBundle("~/bundles/jsKendo").Include( "~/Scripts/Js/kendo/kendo.all.min.js", "~/Scripts/Js/kendo/kendo.angular.min.js", "~/Scripts/Js/kendo/kendo.culture.tr-TR.min.js", "~/Scripts/Js/kendo/kendo.messages.tr-TR.min.js", "~/Scripts/Js/kendo/jszip.min.js"));
            bundles.Add(new StyleBundle("~/bundles/cssKendo").Include("~/Content/css/kendo/kendo.common.min.css", "~/Content/css/kendo/kendo.default.min.css", "~/Content/css/kendo/kendo.default.mobile.min.css"));
            #endregion

            #region Report Bundles
            bundles.Add(new ScriptBundle("~/bundles/Reports/layout").Include(
    "~/Scripts/Reports/layout.js"
));
            #endregion
        }
    }
}
