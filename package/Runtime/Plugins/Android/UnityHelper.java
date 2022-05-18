package com.adsbynimbus.unity;

import static android.view.ViewGroup.LayoutParams.*;

import android.app.Activity;
import android.content.res.Configuration;
import android.view.Gravity;
import android.view.View;
import android.view.ViewGroup;
import android.widget.FrameLayout;

import com.adsbynimbus.NimbusAdManager;
import com.adsbynimbus.NimbusError;
import com.adsbynimbus.openrtb.request.App;
import com.adsbynimbus.openrtb.response.BidResponse;
import com.adsbynimbus.openrtb.request.Format;
import com.adsbynimbus.openrtb.request.Position;
import com.adsbynimbus.openrtb.request.User;
import com.adsbynimbus.render.AdController;
import com.adsbynimbus.render.AdEvent;
import com.adsbynimbus.render.BlockingAdRenderer;
import com.adsbynimbus.render.CompanionAd;
import com.adsbynimbus.render.Renderer;
import com.adsbynimbus.request.NimbusRequest;
import com.adsbynimbus.request.NimbusResponse;
import com.adsbynimbus.request.RequestManager;

import java.util.HashMap;

public final class UnityHelper {
    static final NimbusAdManager manager = new NimbusAdManager();

    static {
        BlockingAdRenderer.setStaticDismissTimeout(10000);
    }

    public static void makeBannerRequest(Object obj, String position, float bannerFloor, float videoFloor, Object listener) {
        if (obj instanceof Activity) {
            final Activity activity = (Activity) obj;
            final NimbusRequest request =
                    NimbusRequest.forBannerAd(position, Format.BANNER_320_50, Position.FOOTER);
            request.request.imp[0].banner.bidfloor = (Float) bannerFloor;
            activity.runOnUiThread(() -> manager.makeRequest(activity, request, (NimbusAdManager.Listener) listener));
        }
    }

    public static void makeInterstitialRequest(Object obj, String position, float bannerFloor, float videoFloor, Object listener) {
        if (obj instanceof Activity) {
            final Activity activity = (Activity) obj;
            final NimbusRequest request = NimbusRequest.forInterstitialAd(position);
            request.request.imp[0].banner.bidfloor = (Float) bannerFloor;
            request.request.imp[0].video.bidfloor = (Float) videoFloor;
            request.setCompanionAds(
                new CompanionAd[]{activity.getResources().getConfiguration().orientation ==
                        Configuration.ORIENTATION_LANDSCAPE ?
                        CompanionAd.end(480, 320) : CompanionAd.end(320, 480)});
            activity.runOnUiThread(() -> manager.makeRequest(activity, request, (NimbusAdManager.Listener) listener));
        }
    }

    public static void makeRewardedVideoRequest(Object obj, String position, float bannerFloor, float videoFloor, Object listener) {
        if (obj instanceof Activity) {
            final Activity activity = (Activity) obj;
            final NimbusRequest request = NimbusRequest.forRewardedVideo(position, 
                activity.getResources().getConfiguration().orientation);
            request.request.imp[0].video.bidfloor = (Float) videoFloor;
            activity.runOnUiThread(() -> manager.makeRequest(activity, request, (NimbusAdManager.Listener) listener));
        }
    }

    public static void render(Object obj, String type, String auctionId, String markup, String network, String placementId,
        int width, int height, byte isInterstitial, byte isMraid, String position, String[] impressionTrackers, String[] clickTrackers,
        int duration, int companionWidth, int companionHeight, int closeButtonDelaySeconds, Object listener) {
        if (obj instanceof Activity) {
            final Activity activity = (Activity) obj;
            final HashMap<String, String[]> trackers = new HashMap<>();
            trackers.put("impression_trackers", impressionTrackers);
            trackers.put("click_trackers", clickTrackers);
            final NimbusResponse response = new NimbusResponse(new BidResponse(type, auctionId, null, 0, 0, null, null, height, width, 
                isInterstitial, markup, network, placementId, isMraid, position, trackers, duration));
            if (companionWidth != 0 && companionHeight != 0) {
                response.companionAds = new CompanionAd[]{ CompanionAd.end(companionWidth, companionHeight) };
            }
            activity.runOnUiThread(new BannerHandler(activity, null, response, (NimbusAdManager.Listener) listener));
        }
    }

    public static void renderBlocking(Object obj, String type, String auctionId, String markup, String network, String placementId,
        int width, int height, byte isInterstitial, byte isMraid, String position, String[] impressionTrackers, String[] clickTrackers,
        int duration, int companionWidth, int companionHeight, int closeButtonDelaySeconds, Object listener) {
        if (obj instanceof Activity) {
            final Activity activity = (Activity) obj;
            final HashMap<String, String[]> trackers = new HashMap<>();
            trackers.put("impression_trackers", impressionTrackers);
            trackers.put("click_trackers", clickTrackers);
            final NimbusResponse response = new NimbusResponse(new BidResponse(type, auctionId, null, 0, 0, null, null, height, width, 
                isInterstitial, markup, network, placementId, isMraid, position, trackers, duration));
            if (companionWidth != 0 && companionHeight != 0) {
                response.companionAds = new CompanionAd[]{ CompanionAd.end(companionWidth, companionHeight) };
            } else {
                response.companionAds = new CompanionAd[]{activity.getResources().getConfiguration().orientation ==
                        Configuration.ORIENTATION_LANDSCAPE ?
                        CompanionAd.end(480, 320) : CompanionAd.end(320, 480)})
            }
            activity.runOnUiThread(() -> {
                BlockingAdRenderer.setsCloseButtonDelayRender(closeButtonDelaySeconds * 1000);
                final AdController controller = Renderer.loadBlockingAd(response, activity);
                final NimbusAdManager.Listener callback = (NimbusAdManager.Listener) listener;
                if (controller != null) {
                    callback.onAdRendered(controller);
                    controller.start();
                 } else {
                    callback.onError(new NimbusError(NimbusError.ErrorType.RENDERER_ERROR, "Error rendering blocking ad", null));
                }
            });
         
        }
    }

    public static void setUser(String gdprConsent) {
        final User user = new User();
        user.ext = new User.Extension();
        user.ext.consent = gdprConsent;
        RequestManager.setUser(user);
    }

    public static void showInterstitialAd(Object obj, String position, float bannerFloor, float videoFloor,
            int closeButtonDelaySeconds, Object listener) {
        if (obj instanceof Activity) {
            final Activity activity = (Activity) obj;
            final NimbusRequest request = NimbusRequest.forInterstitialAd(position);
            request.request.imp[0].banner.bidfloor = (Float) bannerFloor;
            request.request.imp[0].video.bidfloor = (Float) videoFloor;
            request.setCompanionAds(
                new CompanionAd[]{activity.getResources().getConfiguration().orientation ==
                        Configuration.ORIENTATION_LANDSCAPE ?
                        CompanionAd.end(480, 320) : CompanionAd.end(320, 480)});
            activity.runOnUiThread(() -> manager.showBlockingAd(request, activity,
                    (NimbusAdManager.Listener) listener));
        }
    }

    public static void showRewardedVideoAd(Object obj, String position, float bannerFloor, float videoFloor,
            int closeButtonDelaySeconds, Object listener) {
        if (obj instanceof Activity) {
            final Activity activity = (Activity) obj;
            final NimbusRequest request = NimbusRequest.forRewardedVideo(position, 
                activity.getResources().getConfiguration().orientation);
            request.request.imp[0].video.bidfloor = (Float) videoFloor;
            activity.runOnUiThread(() -> manager.showRewardedAd(request, (Integer) closeButtonDelaySeconds, activity,
                (NimbusAdManager.Listener) listener));
        }
    }

    public static void showBannerAd(Object obj, String position, float bannerFloor, float videoFloor,
            int closeButtonDelay, Object listener) {
        if (obj instanceof Activity) {
            final Activity activity = (Activity) obj;
            final NimbusRequest request =
                    NimbusRequest.forBannerAd(position, Format.BANNER_320_50, Position.FOOTER);
            request.request.imp[0].banner.bidfloor = (Float) bannerFloor;
            activity.runOnUiThread(new BannerHandler(activity, request, (NimbusAdManager.Listener) listener));
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

    static final class BannerHandler implements Runnable, NimbusAdManager.Listener,
        AdController.Listener {

        protected final NimbusRequest request;
        protected final NimbusResponse response;
        protected Activity activity;
        protected FrameLayout adFrame;
        protected NimbusAdManager.Listener loadListener;

        public BannerHandler(Activity activity, NimbusRequest request, NimbusAdManager.Listener listener) {
            this.activity = activity;
            this.request = request;
            this.loadListener = listener;
            this.response = null;
        }

        public BannerHandler(Activity activity, NimbusRequest request, NimbusResponse response,
            NimbusAdManager.Listener listener) {
            this.activity = activity;
            this.request = request;
            this.loadListener = listener;
            this.response = response;
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
                if (response == null) manager.showAd(request, adFrame, this); else {
                    Renderer.loadAd(response, adFrame, this);
                }
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