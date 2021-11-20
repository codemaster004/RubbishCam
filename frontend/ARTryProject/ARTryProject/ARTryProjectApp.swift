//
//  ARTryProjectApp.swift
//  ARTryProject
//
//  Created by Filip Dabkowski on 10/11/2021.
//

import SwiftUI

@main
struct ARTryProjectApp: App {
    
    @StateObject var placementSettings = PlacementSettings()
    
    var body: some Scene {
        WindowGroup {
            ContentView()
                .environmentObject(placementSettings)
        }
    }
}
