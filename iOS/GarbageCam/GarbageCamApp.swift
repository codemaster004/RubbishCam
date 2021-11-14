//
//  GarbageCamApp.swift
//  GarbageCam
//
//  Created by Filip Dabkowski on 29/10/2021.
//

import SwiftUI

@main
struct GarbageCamApp: App {
    var body: some Scene {
        WindowGroup {
            ContentView(classifier: ImageClassifier())
        }
    }
}
