//
//  SendMessageInterface.m
//  UnityFramework
//
//  Created by Victor Takai on 07/06/22.
//

#import "SendMessageInterface.h"
#import "UnityInterface.h"

@implementation SendMessageInterface

+ (void) SendMessage:(NSString *)obj to:(NSString*) method and:(NSString*) msg {
    UnitySendMessage([obj UTF8String], [method UTF8String], [msg UTF8String]);
}

@end
