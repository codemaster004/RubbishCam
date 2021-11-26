//
//  ControlView.swift
//  ARTryProject
//
//  Created by Filip Dabkowski on 15/11/2021.
//

import SwiftUI

struct ControlView: View {
    @EnvironmentObject var placementSettings: PlacementSettings
    var models: [Model]
    
    var body: some View {
        VStack {
//            ControlVisibilityToggleButton()
            
            Spacer()
            
            ScrollView(.horizontal, showsIndicators: false) {
                HStack {
                    ForEach(0..<models.count) { index in
                        
                        let model = models[index]
                        
                        ARObjectButton(buttonText: model.name) {
                            model.asyncLoadModelEntity()
                            self.placementSettings.selectedModel = model
                            print("Selected Model: \(model.name)")
                        }
                    }
                }
                .padding(.horizontal)
            }
            
//            ControlButtonBar()
        }
    }
}

struct ControlView_Previews: PreviewProvider {
    static var previews: some View {
        ContentView()
    }
}

struct ControlVisibilityToggleButton: View {
    var body: some View {
        HStack {
            
            Spacer()
            
            ControlButton(systemIconName: "note") {
                print("Browse View button pressed")
            }
            .padding(5)
            .background(Color.black.opacity(0.25))
            .cornerRadius(10)
        }
        
        .padding()
    }
}

struct ControlButtonBar: View {
    var body: some View {
        HStack {
            ControlButton(systemIconName: "clock.fill") {
                print("Most Recent Placed button pressed")
            }
            
            Spacer()
            
            ControlButton(systemIconName: "square.grid.2x2") {
                print("Brows Button Pressed")
            }
            
            Spacer()
            
            ControlButton(systemIconName: "slider.horizontal.3") {
                print("Settings Button Pressed")
            }
            
        }
        .frame(maxWidth: 500)
        .padding(30)
        .background(Color.black.opacity(0.25))
    }
}

struct ControlButton: View {
    let systemIconName: String
    let action: () -> Void
    
    var body: some View {
        Button(action: {
            self.action()
        }) {
            Image(systemName: systemIconName)
                .font(.system(size: 35))
                .foregroundColor(.white)
                .buttonStyle(PlainButtonStyle())
        }
        .frame(width: 50, height: 50)
    }
}

struct ARObjectButton: View {
    
    let buttonText: String
    let action: () -> Void
    
    var body: some View {
        VStack {
            Image(buttonText)
                .resizable()
                .frame(width: 150, height: 150)
            
            Text(buttonText)
                .foregroundColor(.white)
                .font(.system(size: 18))
                .padding(.horizontal)
                .padding(.vertical, 5)
        }
        .background(Color.black.opacity(0.25))
        .cornerRadius(20)
        .onTapGesture {
            self.action()
        }
    }
}
