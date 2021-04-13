package com.nimbus.demo;

import static android.view.ViewGroup.LayoutParams.*;

import android.app.Activity;
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
import com.adsbynimbus.render.AdEvent;
import com.adsbynimbus.render.BlockingAdRenderer;
import com.adsbynimbus.request.NimbusRequest;
import com.adsbynimbus.request.NimbusResponse;
import com.adsbynimbus.request.RequestManager;

public final class UnityHelper {
    static final NimbusAdManager manager = new NimbusAdManager();

    static {
        BlockingAdRenderer.setDismissOnComplete(true);
    }

    public static void setApp(Object bundleId, Object appName, Object domain, Object storeUrl) {
        final App app = new App();
        app.bundle = bundleId.toString();
        app.name = appName.toString();
        app.domain = domain.toString();
        app.storeurl = storeUrl.toString();
        RequestManager.setApp(app);
    }

    public static void showInterstitialAd(Object obj, Object position, Object listener) {
        if (obj instanceof Activity) {
            final Activity activity = (Activity) obj;
            activity.runOnUiThread(() -> manager.showBlockingAd(
                NimbusRequest.forInterstitialAd(position.toString()), activity,
                (NimbusAdManager.Listener) listener));
        }
    }

    public static void showRewardedAd(Object obj, Object position, Object listener) {
        if (obj instanceof Activity) {
            final Activity activity = (Activity) obj;
            activity.runOnUiThread(() -> manager.showRewardedVideoAd(
                NimbusRequest.forInterstitialAd(position.toString()), 5000, activity,
                (NimbusAdManager.Listener) listener));
        }
    }

    public static void showRewardedVideoAd(Object obj, Object position, Object listener) {
        if (obj instanceof Activity) {
            final Activity activity = (Activity) obj;
            activity.runOnUiThread(() -> manager.showRewardedVideoAd(
                NimbusRequest.forRewardedVideo(position.toString()), 5000, activity,
                (NimbusAdManager.Listener) listener));
        }
    }

    public static void showBannerAd(Object obj, Object position, Object listener) {
        if (obj instanceof Activity) {
            final Activity activity = (Activity) obj;

            activity.runOnUiThread(new BannerHandler(activity,
                NimbusRequest.forBannerAd(position.toString(), Format.BANNER_320_50, Position.FOOTER),
                (NimbusAdManager.Listener) listener));
        }
    }

    public static void addListener(Object controller, Object listener) {
        if (controller instanceof AdController) {
            ((AdController) controller).listeners().add((AdController.Listener) listener);
        }
    }
    
    public static void destroyController(Object obj, Object controller) {
        if (obj instanceof Activity) {
            final Activity activity = (Activity) obj;
            if (controller instanceof AdController) {
                activity.runOnUiThread(() -> ((AdController)controller).destroy());
            }
        }
    }

    public static void destroy(Object obj, Object controller) {
        if (obj instanceof Activity) {
            final Activity activity = (Activity) obj;
            if (controller instanceof AdController) {
                activity.runOnUiThread(() -> ((AdController)controller).destroy());
            }
        }
    }

    static final class BannerHandler implements Runnable, NimbusAdManager.Listener,
        AdController.Listener {

        protected final NimbusRequest request;
        protected Activity activity;
        protected FrameLayout adFrame;
        protected NimbusAdManager.Listener loadListener;

        public BannerHandler(Activity activity, NimbusRequest request,
            NimbusAdManager.Listener listener) {
            this.activity = activity;
            this.request = request;
            this.loadListener = listener;
        }

        @Override
        public void run() {
            if (adFrame == null && activity != null) {
                adFrame = new FrameLayout(activity) {
                    @Override
                    public void onViewAdded(View child) {
                        super.onViewAdded(child);
                        ((LayoutParams) child.getLayoutParams()).gravity = Gravity.BOTTOM | Gravity.CENTER_HORIZONTAL;
                    }
                };
                activity.addContentView(adFrame, new ViewGroup.LayoutParams(MATCH_PARENT, MATCH_PARENT));
                manager.showAd(request, adFrame, this);
                activity = null;
            }
        }

        @Override
        public void onError(NimbusError error) {
            if (loadListener != null) {
                loadListener.onError(error);
                loadListener = null;
            }
            if (adFrame != null) {
                if (adFrame.getParent() instanceof ViewGroup) {
                    ((ViewGroup) adFrame.getParent()).removeView(adFrame);
                }
                adFrame = null;
            }
        }

        @Override
        public void onAdResponse(NimbusResponse nimbusResponse) {
            if (loadListener != null) {
                loadListener.onAdResponse(nimbusResponse);
            }
        }

        @Override
        public void onAdRendered(AdController controller) {
            controller.listeners().add(this);
            if (loadListener != null) {
                loadListener.onAdRendered(controller);
                loadListener = null;
            }
        }

        @Override
        public void onAdEvent(AdEvent adEvent) {
            if ((adEvent == AdEvent.DESTROYED || adEvent == AdEvent.COMPLETED) && adFrame != null) {
                if (adFrame.getParent() instanceof ViewGroup) {
                    ((ViewGroup) adFrame.getParent()).removeView(adFrame);
                }
                adFrame = null;
            }
        }
    }
}