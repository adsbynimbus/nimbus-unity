import Foundation

// This globally stubs out the missing C function for your test target scope
@_silgen_name("UnitySendMessage")
public func UnitySendMessage(_ objectName: UnsafePointer<Int8>?, _ methodName: UnsafePointer<Int8>?, _ msg: UnsafePointer<Int8>?) {
    // Left empty intentionally to safely swallow the engine call during unit tests
}