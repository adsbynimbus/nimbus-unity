//
//  AdView.swift
//  Unity-iPhone
//
//  Created by Bruno Bruggemann on 5/26/21.
//

import Foundation
import NimbusKit

class AdView: UIView {
    
    init(bannerFormat: NimbusAdFormat) {
        let rect = CGRect(x: 0, y: 0, width: bannerFormat.width, height: bannerFormat.height)
        super.init(frame: rect)
    }
    
    required init?(coder: NSCoder) {
        fatalError("init(coder:) has not been implemented")
    }
    
    func attachToView(parentView: UIView, position: NimbusPosition) {
        parentView.addSubview(self)
        
        if #available(iOS 11.0, *) {
            translatesAutoresizingMaskIntoConstraints = false
            NSLayoutConstraint.activate([
                widthAnchor.constraint(equalToConstant: frame.width),
                heightAnchor.constraint(equalToConstant: frame.height),
                bottomAnchor.constraint(equalTo: parentView.safeAreaLayoutGuide.bottomAnchor),
                centerXAnchor.constraint(equalTo: parentView.safeAreaLayoutGuide.centerXAnchor)
            ])
        } else {
            let ogFrame = frame
            autoresizingMask = [.flexibleLeftMargin, .flexibleRightMargin, .flexibleTopMargin]
            frame = CGRect(
                x: UIScreen.main.bounds.size.width / 2 - ogFrame.size.width / 2,
                y: UIScreen.main.bounds.size.height - ogFrame.size.height,
                width: ogFrame.width,
                height: ogFrame.height
            )
        }
    }   
}