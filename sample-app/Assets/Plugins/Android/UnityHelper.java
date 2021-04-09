package com.nimbus.demo;

import static android.view.ViewGroup.LayoutParams.*;

import android.app.Activity;
import android.view.Gravity;
import android.view.View;
import android.view.ViewGroup;
import android.widget.FrameLayout;

import com.adsbynimbus.NimbusAdManager;
import com.adsbynimbus.openrtb.request.App;
import com.adsbynimbus.openrtb.request.Format;
import com.adsbynimbus.openrtb.request.Position;
import com.adsbynimbus.render.AdController;
import com.adsbynimbus.render.BlockingAdRenderer;
import com.adsbynimbus.request.NimbusRequest;
import com.adsbynimbus.request.RequestManager;

public class UnityHelper {
    static final NimbusAdManager manager = new NimbusAdManager();

    static {
        BlockingAdRenderer.setDismissOnComplete(true);
    }

    public static void setApp(Object bundleId, Object appName, Object domain) {
        final App app = new App();
        app.bundle = bundleId.toString();
        app.name = appName.toString();
        app.domain = domain.toString();
        app.storeurl = "https://play.google.com/store/apps/details?id=" + bundleId.toString();
        RequestManager.setApp(app);
    }

    public static void showInterstitialAd(Object obj, Object listener) {
        if (obj instanceof Activity) {
            final Activity activity = (Activity) obj;
            activity.runOnUiThread(() -> manager.showBlockingAd(
                    NimbusRequest.forInterstitialAd("test_interstitial"), activity,
                    (NimbusAdManager.Listener) listener));
        }
    }

    public static void showRewardedVideoAd(Object obj, Object listener) {
        if (obj instanceof Activity) {
            final Activity activity = (Activity) obj;
            activity.runOnUiThread(() -> manager.showRewardedVideoAd(
                    NimbusRequest.forRewardedVideo("test_rewarded"), 5000, activity,
                    (NimbusAdManager.Listener) listener));
        }
    }

    public static void showBannerAd(Object obj, Object listener) {
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
                        adFrame, (NimbusAdManager.Listener) listener);
            });
        }
    }

    public static void addListener(Object controller, Object listener) {
        if (controller instanceof AdController) {
            ((AdController) controller).listeners().add((AdController.Listener) listener);
        }
    }
}