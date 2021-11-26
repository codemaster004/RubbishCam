//
//  PlacementSettings.swift
//  ARTryProject
//
//  Created by Filip Dabkowski on 17/11/2021.
//

import SwiftUI
import ReplayKit
import Combine

class PlacementSettings: ObservableObject {
    
    @Published var selectedModel: Model? {
        willSet(newValue) {
            print("Setting selectedModel to \(String(describing: newValue?.name))")
        }
    }
    
    @Published var confirmModel: Model? {
        willSet(newValue) {
            guard let model = newValue else {
                print("Cleaning confirmedModel")
                return
            }
            
            print("Setting confirmedModel to \(model.name)")
        }
    }
    
    var sceneObserver: Cancellable? 
}

