//
//  CurveShape.swift
//  GarbageCamApp
//
//  Created by Filip Dabkowski on 04/12/2021.
//

import Foundation
import SwiftUI

struct AnimatedShape: Shape {
    
    var centerX: CGFloat
    let curveRadius: CGFloat
    let curveHeight: CGFloat
    let curveWidth: CGFloat
    
    var animatableData: CGFloat {
        get { return centerX }
        set { centerX = newValue }
    }
    
    func path(in rect: CGRect) -> Path {
        
        return Path { path in
            path.move(to: CGPoint(x: 0, y: rect.height))
            path.addLine(to: CGPoint(x: 0, y: (curveHeight + curveRadius)))
            path.addQuadCurve(to: CGPoint(x: curveRadius, y: curveHeight), control: CGPoint(x: 0, y: curveHeight))
            path.addLine(to: CGPoint(x: rect.width - curveRadius, y: curveHeight))
            path.addQuadCurve(to: CGPoint(x: rect.width, y: (curveHeight + curveRadius)), control: CGPoint(x: rect.width, y: curveHeight))
            path.addLine(to: CGPoint(x: rect.width, y: rect.height))
            
            
            path.move(to: CGPoint(x: centerX - curveWidth * 2, y: curveHeight))
            
            let to1 = CGPoint(x: centerX, y: 0)
            let control1 = CGPoint(x: centerX - curveWidth, y: curveHeight)
            let control2 = CGPoint(x: centerX - curveWidth, y: 0)
            
            let to2 = CGPoint(x: centerX + curveWidth * 2, y: curveHeight)
            let control3 = CGPoint(x: centerX + curveWidth, y: 0)
            let control4 = CGPoint(x: centerX + curveWidth, y: curveHeight)
            
            path.addCurve(to: to1, control1: control1, control2: control2)
            path.addCurve(to: to2, control1: control3, control2: control4)
//            path.addQuadCurve(to: CGPoint(x: centerX + 35, y: 15), control: CGPoint(x: centerX, y: -30))
        }
    }
}
