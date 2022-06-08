//
//  SendMessageInterface.h
//  UnityFramework
//
//  Created by Victor Takai on 07/06/22.
//

NS_ASSUME_NONNULL_BEGIN

@interface SendMessageInterface : NSObject

+ (void) SendMessage:(NSString *)obj to:(NSString*) method and:(NSString*) msg;

@end

NS_ASSUME_NONNULL_END
