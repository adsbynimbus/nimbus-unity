package com.adsbynimbus.unity

import android.app.Activity
import android.os.Handler
import android.os.Looper
import android.util.Log
import android.util.TypedValue
import android.view.Gravity
import android.view.View
import android.view.ViewGroup
import android.view.ViewGroup.LayoutParams.WRAP_CONTENT
import android.widget.FrameLayout
import androidx.constraintlayout.motion.widget.Key
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat
import androidx.core.view.updateLayoutParams
import androidx.lifecycle.LifecycleCoroutineScope
import androidx.lifecycle.LifecycleOwner
import androidx.lifecycle.coroutineScope
import com.adsbynimbus.InMobiExtension
import com.adsbynimbus.Nimbus
import com.adsbynimbus.bannerAd
import com.adsbynimbus.internal.NimbusExtension
import com.adsbynimbus.internal.lifecycleOrNimbusScope
import com.adsbynimbus.interstitialAd
import com.adsbynimbus.openrtb.enumerations.Position
import com.adsbynimbus.render.AdController
import com.adsbynimbus.request.AdSize
import com.adsbynimbus.rewardedAd
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import java.util.concurrent.ExecutorService
import java.util.concurrent.Executors

class UnityHelper {
    val executor: ExecutorService? = Executors.newSingleThreadExecutor()
    public val LifecycleOwner.lifecycleScope: LifecycleCoroutineScope
        get() = lifecycle.coroutineScope
    companion object {

        @JvmStatic
        fun initNimbusAndThirdParties(obj: Any?, publisherKey: String, apiKey: String) {
            if (obj is Activity) {
                val extensions = mutableSetOf<NimbusExtension>()
                extensions.add(InMobiExtension("f5ccf534fc594454b1a21138527cc47b"))
                Nimbus.initialize(obj, publisherKey, apiKey, extensions)
            }
        }

        @JvmStatic
        fun render(
            obj: Any?,
            isBlocking: Boolean,
            isRewarded: Boolean,
            closeButtonDelay: Int,
            listener: Any?,
            mintegralAdUnitId: String?,
            mintegralAdUnitPlacementId: String?,
            molocoAdUnitId: String?,
            inMobiPlacementId: String?,
            respectSafeArea: Boolean,
            adPosition: Int
        ) {
            if (obj is Activity) {
                Nimbus.configuration.requestUrl = "https://dev-sdk.adsbynimbus.com/rta/test"
                val ad = Nimbus.interstitialAd("unity_test").onEvent { event ->
                    Log.d("WTF", "WTF$event")
                }.onError {error ->  Log.d("WTF", "LOL", error) }
                val scope = CoroutineScope(Dispatchers.Main)
                scope.launch {
                    val adframe = FrameLayout(obj)
                    obj.addContentView(adframe, ViewGroup.LayoutParams(
                        ViewGroup.LayoutParams.MATCH_PARENT,
                        ViewGroup.LayoutParams.MATCH_PARENT
                    ))
                    /*val adr = Nimbus.bannerAd(position = "", size = AdSize.Banner)
                        .onEvent {
                            Log.d("Event", it.toString(), )
                        }.onError {
                            Log.d("ERROR", "ERROR", it)
                        }.show(adframe).also {
                            it.adView?.updateLayoutParams<FrameLayout.LayoutParams> {
                                gravity = Gravity.TOP or Gravity.CENTER_HORIZONTAL
                                height = WRAP_CONTENT
                            }
                        }*/
                    ad.show(obj)
                }
            }
        }

    }

    fun addListener(controller: Any?, listener: Any?) {
        if (controller is AdController) {
            controller.listeners()
                .add((listener as com.adsbynimbus.render.AdController.Listener?)!!)
        }
    }

    fun destroyController(obj: Any?, controller: Any?) {
        if (obj is Activity) {
            val activity = obj
            if (controller is AdController) {
                activity.runOnUiThread(Runnable { controller.destroy() })
            }
        }
    }

    internal class BannerHandler {
        protected var activity: Activity?
        protected var adFrame: ViewGroup? = null
        protected var respectSafeArea: Boolean = false

        protected var adPosition: Int = 0

        protected var width: Int = 0

        protected var height: Int = 0

        constructor(
            activity: Activity?, width: Int, height: Int, respectSafeArea: Boolean, adPosition: Int
        ) {
            this.activity = activity
            this.width = width
            this.height = height
            this.respectSafeArea = respectSafeArea
            this.adPosition = adPosition
        }

        fun getBannerFrame(): ViewGroup? {
            if (adFrame == null && activity != null) {
                adFrame = object : ViewGroup(activity!!) {
                    override fun onLayout(
                        p0: Boolean,
                        p1: Int,
                        p2: Int,
                        p3: Int,
                        p4: Int
                    ) {

                    }

                    override fun onViewAdded(child: View) {
                        super.onViewAdded(child)
                        if (respectSafeArea) {
                            ViewCompat.setOnApplyWindowInsetsListener(
                                child,
                                androidx.core.view.OnApplyWindowInsetsListener { v: View?, windowInsets: WindowInsetsCompat? ->
                                    val insets =
                                        windowInsets!!.getInsets(WindowInsetsCompat.Type.systemBars())
                                            .toPlatformInsets()
                                    // Apply the insets as a margin to the view. This solution sets only the
                                    // bottom, left, and right dimensions, but you can apply whichever insets are
                                    // appropriate to your layout. You can also update the view padding if that's
                                    // more appropriate.
                                    val mlp = v!!.getLayoutParams() as MarginLayoutParams
                                    mlp.leftMargin = insets.left
                                    mlp.bottomMargin = insets.bottom
                                    mlp.rightMargin = insets.right
                                    v.setLayoutParams(mlp)
                                    WindowInsetsCompat.CONSUMED
                                })
                        }
                        var gravity = 0
                        when (adPosition) {
                            0 -> gravity = Gravity.BOTTOM or Gravity.CENTER_HORIZONTAL
                            1 -> gravity = Gravity.TOP or Gravity.CENTER_HORIZONTAL
                            2 -> gravity = Gravity.CENTER
                            3 -> gravity = Gravity.BOTTOM or Gravity.START
                            4 -> gravity = Gravity.BOTTOM or Gravity.END
                            5 -> gravity = Gravity.TOP or Gravity.START
                            6 -> gravity = Gravity.TOP or Gravity.END
                        }
                        if (width != 0) child.getLayoutParams().width =
                            TypedValue.applyDimension(
                                TypedValue.COMPLEX_UNIT_DIP,
                                width.toFloat(),
                                getResources().getDisplayMetrics()
                            ).toInt()
                        if (height != 0) child.getLayoutParams().height =
                            TypedValue.applyDimension(
                                TypedValue.COMPLEX_UNIT_DIP,
                                height.toFloat(),
                                getResources().getDisplayMetrics()
                            ).toInt()
                        //(child.getLayoutParams() as LayoutParams).gravity = gravity
                    }
                }

                activity!!.addContentView(
                    adFrame,
                    ViewGroup.LayoutParams(
                        ViewGroup.LayoutParams.MATCH_PARENT,
                        ViewGroup.LayoutParams.MATCH_PARENT
                    )
                )
            }
            return adFrame
        }
    }
}