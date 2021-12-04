//
//  ProfileView.swift
//  GarbageCamApp
//
//  Created by Filip Dabkowski on 29/11/2021.
//

import SwiftUI

struct ProfileView: View {
    
    let width = UIScreen.main.bounds.size.width
    let height = UIScreen.main.bounds.size.height
    
    var body: some View {
        ZStack(alignment: .top) {
                
            VStack(spacing: 25) {
                Image("Logo")
                    .resizable()
                    .frame(width: 150, height: 150)
                    .cornerRadius(90)
                
                VStack(spacing: 10) {
                    Text("Maja Kowalska")
                        .fontWeight(.bold)
                        .font(.system(size: 21))
                    
                    Text("Mega Eco-woman")
                        .font(.system(size: 12))
                        .foregroundColor(Color.textGray2)
                }
                
                HStack {
                    UserInfo(value: "200", description: "Found Garbage")
                    UserInfo(value: "56", description: "E-Points")
                    UserInfo(value: "4", description: "Level")
                    UserInfo(value: "2", description: "Days Playing")
                }
                
            }
            .frame(width: width, height: 300)
            .padding(.bottom, 200)
            .background(Color.white)
            
            ScrollView(.vertical, showsIndicators: false) {
                VStack {
                    Spacer()
                        .frame(minHeight: 300)
                    
                    VStack {
                        Text("Hello")
                    }
                    .frame(width: width, height: height - 340)
                    .background(Color.mainColor.clipShape(
                        AnimatedShape(centerX: width / 2, curveRadius: 20, curveHeight: 30, curveWidth: 30))
                    )
                }
            }
            .ignoresSafeArea(.all, edges: .bottom)
        }
        .background(Color.mainColor)
    }
}

struct ProfileViewPreviews: PreviewProvider {
    static var previews: some View {
        ProfileView()
    }
}

struct UserInfo: View {
    
    let value: String
    let description: String
    
    var body: some View {
        VStack {
            Text(value)
                .foregroundColor(.textMain)
                .fontWeight(.semibold)
                .font(.system(size: 18))
            
            Text(description)
                .foregroundColor(.textGray2)
                .fontWeight(.semibold)
                .font(.system(size: 11))
        }
        .frame(width: 90)
    }
}
