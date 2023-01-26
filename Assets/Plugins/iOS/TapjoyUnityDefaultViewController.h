#import <UIKit/UIKit.h>
#import <Tapjoy/Tapjoy.h>
#import "UnityViewControllerBase.h"

#define TJ_ORIENTATION_PROPERTIES \
@property (nonatomic, assign) UIInterfaceOrientation lockedOrientation; \
@property (assign, nonatomic) BOOL canRotate;

@interface TapjoyUnityDefaultViewController : UnityDefaultViewController <TJCTopViewControllerProtocol>
TJ_ORIENTATION_PROPERTIES
@end

@interface TapjoyUnityLandscapeLeftOnlyViewController : UnityLandscapeLeftOnlyViewController <TJCTopViewControllerProtocol>
TJ_ORIENTATION_PROPERTIES
@end

@interface TapjoyUnityLandscapeRightOnlyViewController : UnityLandscapeRightOnlyViewController <TJCTopViewControllerProtocol>
TJ_ORIENTATION_PROPERTIES
@end

@interface TapjoyUnityPortraitOnlyViewController : UnityPortraitOnlyViewController <TJCTopViewControllerProtocol>
TJ_ORIENTATION_PROPERTIES
@end

// Currently our iOS SDK does not support Portrait Upside Down as the only orientation.
// This class should help with futureproofing if we decide to revisit that behavior.
@interface TapjoyUnityPortraitUpsideDownOnlyViewController : UnityPortraitUpsideDownOnlyViewController <TJCTopViewControllerProtocol>
TJ_ORIENTATION_PROPERTIES
@end
