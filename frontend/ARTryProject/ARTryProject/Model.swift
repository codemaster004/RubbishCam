//
//  Model.swift
//  ARTryProject
//
//  Created by Filip Dabkowski on 15/11/2021.
//

import SwiftUI
import ReplayKit
import Combine
import RealityKit

class Model {
    var name: String
    var modelEntity: ModelEntity?
    var scaleCompensation: Float
    
    private var cancellable: AnyCancellable?
    
    init(name: String, scale: Float = 1.0) {
        self.name = name
        self.scaleCompensation = scale
    }
    
    func asyncLoadModelEntity() {
        let filename = self.name + ".usdz"
        
        self.cancellable = ModelEntity.loadModelAsync(named: filename)
            .sink { loadCompletion in
                
                switch loadCompletion {
                case .failure(let error): print("Unable to load modelEntity from \(filename). Error: \(error.localizedDescription)")
                case .finished: break
                }
            } receiveValue: { modelEntity in
                self.modelEntity = modelEntity
                self.modelEntity?.scale *= self.scaleCompensation
                
                print("modelEntity for \(self.name) has been loaded.")
            }

    }
}
