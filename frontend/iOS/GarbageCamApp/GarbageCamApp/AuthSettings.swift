//
//  AuthSettings.swift
//  GarbageCamApp
//
//  Created by Filip Dabkowski on 27/11/2021.
//

import SwiftUI
import Combine


class AuthState: ObservableObject {
    
    @Published var authView: AuthViews? {
        willSet(newValue) {
            print("Show \(String(describing: newValue)) View")
        }
    }
    
}

enum AuthViews {
    case login
    case signup
    case none
}
