package com.adsbynimbus.unity;

import static android.util.TypedValue.COMPLEX_UNIT_DIP;
import static android.view.ViewGroup.LayoutParams.*;

import static com.adsbynimbus.internal.Logger.log;
import static com.adsbynimbus.request.internal.NimbusSessionExtension.incrementedSignal;

import android.app.Activity;
import android.content.Context;
import android.content.SharedPreferences;
import android.content.res.Configuration;
import android.graphics.Insets;
import android.os.Handler;
import android.os.Looper;
import android.util.DisplayMetrics;
import android.util.Log;
import android.util.TypedValue;
import android.view.Gravity;
import android.view.View;
import android.view.ViewGroup;
import android.widget.FrameLayout;

import androidx.core.view.ViewCompat;
import androidx.core.view.WindowInsetsCompat;

import com.adsbynimbus.Nimbus;
import com.adsbynimbus.NimbusAdManager;
import com.adsbynimbus.NimbusError;
import com.adsbynimbus.openrtb.request.App;
import com.adsbynimbus.openrtb.request.Signals;
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

import kotlinx.serialization.json.Json;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Objects;
import java.util.concurrent.Callable;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.Future;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.TimeoutException;

public final class UnityHelper {
    static final NimbusAdManager manager = new NimbusAdManager();
    static final ExecutorService executor = Executors.newSingleThreadExecutor();
    public static void render(Object obj, String jsonResponse, boolean isBlocking, boolean isRewarded, int closeButtonDelay, Object listener,
            String mintegralAdUnitId, String mintegralAdUnitPlacementId, String molocoAdUnitId, String inMobiPlacementId, boolean respectSafeArea,
                              int adPosition) {
        if (obj instanceof Activity) {
            final Activity activity = (Activity) obj;
            final NimbusResponse nimbusResponse = new NimbusResponse(BidResponse.fromJson(jsonResponse));
            if (mintegralAdUnitId != "") {
                nimbusResponse.renderInfoOverride.put("adUnitId", mintegralAdUnitId);
                nimbusResponse.renderInfoOverride.put("placement", mintegralAdUnitPlacementId);
            }
            if (molocoAdUnitId != "") {
                nimbusResponse.renderInfoOverride.put("moloco_ad_unit_id", molocoAdUnitId);
            }
            if (inMobiPlacementId != "") {
                nimbusResponse.renderInfoOverride.put("inmobi_placement_id", inMobiPlacementId);
            }
            nimbusResponse.renderInfoOverride.put("is_rewarded", String.valueOf(isRewarded));
            if (isBlocking) {
                nimbusResponse.companionAds = new CompanionAd[]{activity.getResources().getConfiguration().orientation ==
                        Configuration.ORIENTATION_LANDSCAPE ?
                        CompanionAd.Companion.end(480, 320) : CompanionAd.Companion.end(320, 480)};

                activity.runOnUiThread(() -> {
                    BlockingAdRenderer.setsCloseButtonDelayRender(closeButtonDelay * 1000);
                    final AdController controller = Renderer.loadBlockingAd(nimbusResponse, activity);
                    final NimbusAdManager.Listener callback = (NimbusAdManager.Listener) listener;
                    if (controller != null) {
                        callback.onAdRendered(controller);
                        controller.start();
                    } else {
                        callback.onError(new NimbusError(NimbusError.ErrorType.RENDERER_ERROR, "Error rendering blocking ad", null));
                    }
                });
            } else {
                activity.runOnUiThread(new BannerHandler(activity, null, nimbusResponse, (NimbusAdManager.Listener) listener, respectSafeArea, adPosition));
            }
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
    
    public static String getPrivacyStrings(Object obj) {
         final String gdprApplies = "IABTCF_gdprApplies";
         final String usPrivacyString = "IABUSPrivacy_String";
         final String gppString = "IABGPP_HDR_GppString";
         final String gppSidString = "IABGPP_GppSID";
         if (obj instanceof Activity) {
             final Activity activity = (Activity) obj;
             SharedPreferences sharedPreferences = activity.getPreferences(Context.MODE_PRIVATE);
             JSONObject regExt = new JSONObject();
             try {
                 regExt.put("gdprApplies", sharedPreferences.getString(gdprApplies, ""));
             } catch (JSONException e) {
                 Log.e("Nimbus Privacy Error", "Unable to retrieve GDPR Enabled String");
             }
             try {
                 regExt.put("usPrivacyString", sharedPreferences.getString(usPrivacyString, ""));
             } catch (JSONException e) {
                 Log.e("Nimbus Privacy Error", "Unable to retrieve US Privacy String");
             }
             try {
                 regExt.put("gppConsentString", sharedPreferences.getString(gppString, ""));
             } catch (JSONException e) {
                 Log.e("Nimbus Privacy Error", "Unable to retrieve GPP Consent String");
             }
             try {
                 regExt.put("gppSectionId", sharedPreferences.getString(gppSidString, ""));
             } catch (JSONException e) {
                 Log.e("Nimbus Privacy Error", "Unable to retrieve GPP Section ID");
             }
             if (regExt.equals(new JSONObject())) {
                 return "";
             }
             return regExt.toString();
         }
             return "";
         }
    public static String getSessionInfo() {
        return Json.Default.encodeToString(Signals.Companion.serializer(), incrementedSignal());
    }

    static final class BannerHandler implements Runnable, NimbusAdManager.Listener,
        AdController.Listener {

        protected final NimbusRequest request;
        protected final NimbusResponse response;
        protected Activity activity;
        protected FrameLayout adFrame;
        protected NimbusAdManager.Listener loadListener;
        protected boolean respectSafeArea;

        protected int adPosition;

        public BannerHandler(Activity activity, NimbusRequest request, NimbusAdManager.Listener listener) {
            this.activity = activity;
            this.request = request;
            this.loadListener = listener;
            this.response = null;
        }

        public BannerHandler(Activity activity, NimbusRequest request, NimbusResponse response,
            NimbusAdManager.Listener listener, boolean respectSafeArea, int adPosition) {
            this.activity = activity;
            this.request = request;
            this.loadListener = listener;
            this.response = response;
            this.respectSafeArea = respectSafeArea;
            this.adPosition = adPosition;
        }

        @Override
        public void run() {
            if (adFrame == null && activity != null) {
                adFrame = new FrameLayout(activity) {
                    @Override
                    public void onViewAdded(View child) {
                        super.onViewAdded(child);
                        if (respectSafeArea) {
                            ViewCompat.setOnApplyWindowInsetsListener(child, (v, windowInsets) -> {
                                Insets insets = windowInsets.getInsets(WindowInsetsCompat.Type.systemBars()).toPlatformInsets();
                                // Apply the insets as a margin to the view. This solution sets only the
                                // bottom, left, and right dimensions, but you can apply whichever insets are
                                // appropriate to your layout. You can also update the view padding if that's
                                // more appropriate.
                                MarginLayoutParams mlp = (MarginLayoutParams) v.getLayoutParams();
                                mlp.leftMargin = insets.left;
                                mlp.bottomMargin = insets.bottom;
                                mlp.rightMargin = insets.right;
                                v.setLayoutParams(mlp);

                                // Return CONSUMED if you don't want the window insets to keep passing
                                // down to descendant views.
                                return WindowInsetsCompat.CONSUMED;
                            });
                        }
                        int gravity = 0;
                        switch (adPosition) {
                            // Bottom Center
                            case 0:
                                gravity = Gravity.BOTTOM | Gravity.CENTER_HORIZONTAL;
                                break;
                            // Top Center
                            case 1:
                                gravity = Gravity.TOP | Gravity.CENTER_HORIZONTAL;
                                break;
                            // Center
                            case 2: 
                                gravity = Gravity.CENTER;
                                break;
                            // Bottom Left
                            case 3:
                                gravity = Gravity.BOTTOM | Gravity.START;
                                break;
                            // Bottom Right
                            case 4:
                                gravity = Gravity.BOTTOM | Gravity.END;
                                break;
                            // Top Left
                            case 5:
                                gravity = Gravity.TOP | Gravity.START;
                                break;
                            // Top Right
                            case 6:
                                gravity = Gravity.TOP | Gravity.END;
                                break;
                                
                        }
                        if (response.width() != 0) child.getLayoutParams().width = (int) TypedValue.applyDimension(COMPLEX_UNIT_DIP, response.width(), getResources().getDisplayMetrics());
                        if (response.height() != 0) child.getLayoutParams().height = (int) TypedValue.applyDimension(COMPLEX_UNIT_DIP, response.height(), getResources().getDisplayMetrics());
                        ((LayoutParams) child.getLayoutParams()).gravity = gravity;
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