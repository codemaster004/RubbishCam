//
//  ContentView.swift
//  ARTryProject
//
//  Created by Filip Dabkowski on 10/11/2021.
//

import SwiftUI
import RealityKit
import Combine

struct ContentView: View {
    
    @EnvironmentObject var placementSettings: PlacementSettings
    
    let models = [
        Model(name: "Gabka"),
        Model(name: "AirForce"),
        Model(name: "ToyRobotVintage"),
    ]
    
    var body: some View {
        ZStack(alignment: .bottom) {
            ARViewContainer()
                .edgesIgnoringSafeArea(.all)

            if placementSettings.selectedModel == nil {
                ControlView(models: models)
            } else {
                PlacementView()
            }

        }
//        .edgesIgnoringSafeArea(.bottom)
    }
}

struct ARViewContainer: UIViewRepresentable {
    @EnvironmentObject var placementSettings: PlacementSettings
    
    func makeUIView(context: Context) -> CustomARView {
        let arView = CustomARView(frame: .zero)
        
        self.placementSettings.sceneObserver = arView.scene.subscribe(to: SceneEvents.Update.self, { (event) in
            
            self.updateScene(for: arView)
        })
        
        return arView
    }
    
    func updateUIView(_ uiView: CustomARView, context: Context) {}
    
    private func updateScene(for arView: CustomARView) {
        arView.focusEntity?.isEnabled = self.placementSettings.selectedModel != nil
        
        if let confirmedModel = self.placementSettings.confirmModel, let modelEntity = confirmedModel.modelEntity {
            
            self.place(modelEntity, in: arView)
            
            self.placementSettings.confirmModel = nil
        }
    }
    
    private func place(_ modelEntity: ModelEntity, in arView: ARView) {
        let clonedEntity = modelEntity.clone(recursive: true)
        
        clonedEntity.generateCollisionShapes(recursive: true)
        arView.installGestures([.translation, .rotation], for: clonedEntity)
        
        let anchorEntity = AnchorEntity(plane: .any)
        
        anchorEntity.addChild(clonedEntity)
        
        arView.scene.addAnchor(anchorEntity)
        
        print("Added modelEntity to scene.")
    }
}

struct ContentView_Previews: PreviewProvider {
    static var previews: some View {
        ContentView()
    }
}
