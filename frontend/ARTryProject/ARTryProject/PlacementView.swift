//
//  PlacementView.swift
//  ARTryProject
//
//  Created by Filip Dabkowski on 19/11/2021.
//

import SwiftUI

struct PlacementView: View {
    
    @EnvironmentObject var placementSettings: PlacementSettings
    
    var body: some View {
        HStack {
            
            Spacer()
            
            PlacementButton(iconName: "xmark.circle.fill") {
                print("Cancel Placement")
                self.placementSettings.selectedModel = nil
            }
            
            Spacer()
            
            PlacementButton(iconName: "checkmark.circle.fill") {
                print("Confirm Placement")
                
                self.placementSettings.confirmModel = self.placementSettings.selectedModel
                
                self.placementSettings.selectedModel = nil
            }
            
            Spacer()
            
        }
        .padding(.bottom, 30)
    }
}

struct PlacementButton: View {
    
    let iconName: String
    let action: () -> Void
    
    var body: some View {
        Button(action: {
            self.action()
        }) {
            Image(systemName: iconName)
                .font(.system(size: 50, weight: .light, design: .default))
                .foregroundColor(.white)
                .buttonStyle(PlainButtonStyle())
        }
        .frame(width: 75, height: 75)
    }
}

struct PlacementView_Previews: PreviewProvider {
    static var previews: some View {
        PlacementView()
    }
}
