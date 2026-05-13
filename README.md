# nimbus-unity

The latest [package](package) can be downloaded from the [Releases](https://github.com/adsbynimbus/nimbus-unity/releases) page

## Minimum Requirements To Play Demo Scenes

- Unity 2021.3


## To install the Ads By Nimbus Package
- Download the Zip file from the release page
- Unzip the Zipped file
- Open the Unity Package Manager
- Click On the + on the top left of the Package Manager
- Select Install From Disk
- Navigate into the unzipped folder and open the package.json file


## CocoaPods

> **Most users can skip this section.** The Nimbus package generates a working Podfile automatically. This section only applies if you're integrating other native iOS dependencies (your own pods, third-party SDKs distributed via CocoaPods, etc.) alongside Nimbus.

### Adding your own pods

If you maintain your own native iOS code or integrate other SDKs via CocoaPods, you can add them to the generated Podfile. A few constraints to be aware of:

**Do not add `use_frameworks!` globally.** The Nimbus iOS SDK and its adapter pods include a mix of static and dynamic frameworks. Adding `use_frameworks!` will cause duplicate class warnings at runtime (`objc[xxxx]: Class X is implemented in both ...`) and may cause crashes, because static frameworks end up linked into both the `Unity-iPhone` and `UnityFramework` targets.

```ruby
# ❌ Don't do this
use_frameworks!

target 'Unity-iPhone' do
  pod 'NimbusSDK'
  pod 'YourCustomPod'
end
```

**If your pod requires dynamic framework integration**, use per-pod linkage instead:

```ruby
target 'Unity-iPhone' do
  pod 'NimbusSDK'
  pod 'YourCustomPod', :linkage => :dynamic

  target 'UnityFramework' do
    inherit! :search_paths
  end
end
```

**Preserve the nested `UnityFramework` target** with `inherit! :search_paths`. This is required for Unity's split target architecture — without it, your Swift imports will fail to compile or you'll get duplicate symbols at link time.

### Troubleshooting

**Duplicate class warnings at app launch** → check for `use_frameworks!` in your Podfile and remove it.

**"No such module 'NimbusKit'"** in UnityFramework → ensure the nested `target 'UnityFramework' do; inherit! :search_paths; end` block is present.
