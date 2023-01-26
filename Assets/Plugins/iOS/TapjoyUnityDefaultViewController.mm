#import "TapjoyUnityDefaultViewController.h"

#define TJ_ORIENTATION_IMP \
- (id)init{ \
    if (self = [super init]) { \
        _lockedOrientation = TJ_DEFAULT_ORIENTATION; \
        _canRotate = TJ_DEFAULT_CAN_ROTATE; \
    } \
    return self; \
} \
\
- (void)setLockedOrientation:(UIInterfaceOrientation)supportedOrientation { \
    _lockedOrientation = supportedOrientation; \
    if (@available(iOS 16.0, *)) { \
        [UIView performWithoutAnimation:^{ \
            SEL selector = @selector(setNeedsUpdateOfSupportedInterfaceOrientations); \
            IMP implementation = [super methodForSelector:selector]; \
            void (*func)(id, SEL) = (void (*)(id, SEL))implementation; \
            func(self, selector); \
        }]; \
    } \
} \
\
- (UIInterfaceOrientationMask)supportedInterfaceOrientations { \
    if (self.lockedOrientation == UIInterfaceOrientationUnknown) { \
        return [super supportedInterfaceOrientations]; \
    } \
    UIInterfaceOrientation interfaceOrientation; \
    if (@available(iOS 13, *)) { \
        interfaceOrientation = self.view.window.windowScene.interfaceOrientation; \
    } else { \
        interfaceOrientation = UIApplication.sharedApplication.statusBarOrientation; \
    } \
    if (interfaceOrientation != UIInterfaceOrientationPortraitUpsideDown) { \
        return (1 << interfaceOrientation); \
    } \
    return [super supportedInterfaceOrientations]; \
} \
\
- (BOOL)shouldAutorotate { \
    if (@available(iOS 16, *)) { \
        return self.lockedOrientation == UIInterfaceOrientationUnknown; \
    } else { \
        return self.canRotate; \
    } \
}

#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wundeclared-selector"
@implementation TapjoyUnityDefaultViewController
#define TJ_DEFAULT_ORIENTATION UIInterfaceOrientationUnknown
#define TJ_DEFAULT_CAN_ROTATE YES

TJ_ORIENTATION_IMP

#undef TJ_DEFAULT_ORIENTATION
#undef TJ_DEFAULT_CAN_ROTATE
@end

@implementation TapjoyUnityLandscapeLeftOnlyViewController
#define TJ_DEFAULT_ORIENTATION UIInterfaceOrientationLandscapeLeft
#define TJ_DEFAULT_CAN_ROTATE NO

TJ_ORIENTATION_IMP

#undef TJ_DEFAULT_ORIENTATION
#undef TJ_DEFAULT_CAN_ROTATE
@end

@implementation TapjoyUnityLandscapeRightOnlyViewController
#define TJ_DEFAULT_ORIENTATION UIInterfaceOrientationLandscapeRight
#define TJ_DEFAULT_CAN_ROTATE NO

TJ_ORIENTATION_IMP

#undef TJ_DEFAULT_ORIENTATION
#undef TJ_DEFAULT_CAN_ROTATE
@end

@implementation TapjoyUnityPortraitOnlyViewController
#define TJ_DEFAULT_ORIENTATION UIInterfaceOrientationPortrait
#define TJ_DEFAULT_CAN_ROTATE NO

TJ_ORIENTATION_IMP

#undef TJ_DEFAULT_ORIENTATION
#undef TJ_DEFAULT_CAN_ROTATE
@end

@implementation TapjoyUnityPortraitUpsideDownOnlyViewController
#define TJ_DEFAULT_ORIENTATION UIInterfaceOrientationPortraitUpsideDown
#define TJ_DEFAULT_CAN_ROTATE NO

TJ_ORIENTATION_IMP

#undef TJ_DEFAULT_ORIENTATION
#undef TJ_DEFAULT_CAN_ROTATE
@end
#pragma clang diagnostic pop
