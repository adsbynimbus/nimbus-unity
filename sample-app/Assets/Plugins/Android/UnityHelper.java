package com.nimbus.demo;

import static android.view.ViewGroup.LayoutParams.*;

import android.app.Activity;
import android.util.Log;
import android.view.Gravity;
import android.view.View;
import android.view.ViewGroup;
import android.widget.FrameLayout;

import com.adsbynimbus.NimbusAdManager;
import com.adsbynimbus.NimbusError;
import com.adsbynimbus.openrtb.request.App;
import com.adsbynimbus.openrtb.request.Format;
import com.adsbynimbus.openrtb.request.Position;
import com.adsbynimbus.render.AdController;
import com.adsbynimbus.request.NimbusRequest;
import com.adsbynimbus.request.NimbusResponse;
import com.adsbynimbus.request.RequestManager;

public class UnityHelper {
    static final NimbusAdManager manager = new NimbusAdManager();

    public static void setApp(Object bundleId, Object appName, Object domain) {
        final App app = new App();
        app.bundle = bundleId.toString();
        app.name = appName.toString();
        app.domain = domain.toString();
        app.storeurl = "https://play.google.com/store/apps/details?id=" + bundleId.toString();
        RequestManager.setApp(app);
    }

    public static void showInterstitialAd(Object obj) {
        if (obj instanceof Activity) {
            final Activity activity = (Activity) obj;
            activity.runOnUiThread(() -> manager.showBlockingAd(
                    NimbusRequest.forInterstitialAd("test_interstitial"), activity, LoggingListener));
        }
    }

    public static void showRewardedVideoAd(Object obj) {
        if (obj instanceof Activity) {
            final Activity activity = (Activity) obj;
            activity.runOnUiThread(() -> manager.showRewardedVideoAd(
                    NimbusRequest.forRewardedVideo("test_rewarded"), 5000, activity, LoggingListener));
        }
    }

    public static void showBannerAd(Object obj) {
        if (obj instanceof Activity) {
            final Activity activity = (Activity) obj;
            activity.runOnUiThread(() -> {
                final FrameLayout adFrame = new FrameLayout(activity) {
                    @Override
                    public void onViewAdded(View child) {
                        super.onViewAdded(child);
                        ((LayoutParams) child.getLayoutParams()).gravity = Gravity.BOTTOM | Gravity.CENTER_HORIZONTAL;
                    }
                };
                activity.addContentView(adFrame, new ViewGroup.LayoutParams(MATCH_PARENT, MATCH_PARENT));
                manager.showAd(NimbusRequest.forBannerAd("test_banner", Format.BANNER_320_50, Position.FOOTER),
                        adFrame, LoggingListener);
            });
        }
    }

    public static final NimbusAdManager.Listener LoggingListener = new NimbusAdManager.Listener() {
        @Override
        public void onAdRendered(AdController controller) {
            Log.i("Nimbus", "Ad Rendered");
        }

        @Override
        public void onError(NimbusError error) {
            Log.e("Error", error.getMessage());
        }

        @Override
        public void onAdResponse(NimbusResponse nimbusResponse) {
            Log.i("Nimbus", "Successful response " + nimbusResponse.auction_id);
        }
    };
}