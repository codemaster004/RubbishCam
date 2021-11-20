//
//  RecogniseFunctions.swift
//  GarbageCam
//
//  Created by Filip Dabkowski on 29/10/2021.
//

import Foundation
import SwiftUI
import CoreML
import Vision
import CoreImage


struct Classifier {
    
    private(set) var results: String?
    
    mutating func detect(ciImage: CIImage) {
        
        guard let model = try? VNCoreMLModel(for: GarbaObjectDetection(configuration: MLModelConfiguration()).model)
        else {
            print("Return")
            return
        }
        
        let request = VNCoreMLRequest(model: model)
        
        let handler = VNImageRequestHandler(ciImage: ciImage, options: [:])
        
        try? handler.perform([request])
//        print(request.results)
        
        guard let results = request.results as? [VNRecognizedObjectObservation] else {
            print("No results")
            return
        }
//        print(results)
//        print(results.first?.labels.first)
        
//        for recognizedObject in results {
//            print(recognizedObject)
//        }
        
        var recognitions: [String] = []
        for recognition in results {
            if recognition.labels.count > 0 {
                recognitions.append(recognition.labels[0].identifier)
            }
        }
        
        self.results = recognitions.joined(separator: ", ")
//        if let firstResult = results.first {
//            print(firstResult.labels[0].identifier)
//            self.results = firstResult.labels[0].identifier
//        }
        
    }
    
}
