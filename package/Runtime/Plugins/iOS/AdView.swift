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
            self.translatesAutoresizingMaskIntoConstraints = false
            
            var constraints = [self.widthAnchor.constraint(equalToConstant: self.frame.width),
                               self.heightAnchor.constraint(equalToConstant: self.frame.height)]
            
            constraints.append(contentsOf: [self.bottomAnchor .constraint(equalTo: parentView.safeAreaLayoutGuide.bottomAnchor)])
            constraints.append(contentsOf: [self.centerXAnchor .constraint(equalTo: parentView.safeAreaLayoutGuide.centerXAnchor)])
            
            NSLayoutConstraint.activate(constraints)
        } else {
            var rect = self.frame
            
            let screenWidth = UIScreen.main.bounds.size.width
            let screenHeight = UIScreen.main.bounds.size.height
            
            rect.origin.x = screenWidth / 2 - rect.size.width / 2
            rect.origin.y = screenHeight - rect.size.height
            self.autoresizingMask = [.flexibleLeftMargin, .flexibleRightMargin, .flexibleTopMargin]
            self.frame = rect
        }
    }
    
}
