//
//  GarbageCamAppApp.swift
//  GarbageCamApp
//
//  Created by Filip Dabkowski on 21/11/2021.
//

import SwiftUI

@main
struct GarbageCamAppApp: App {
    
    @StateObject var authState = AuthState()
    
    var body: some Scene {
        WindowGroup {
            ContentView()
                .environmentObject(authState)
        }
    }
}
