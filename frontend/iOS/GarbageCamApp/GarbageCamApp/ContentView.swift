//
//  ContentView.swift
//  GarbageCamApp
//
//  Created by Filip Dabkowski on 21/11/2021.
//

import SwiftUI

struct ContentView: View {
    
    @EnvironmentObject var authState: AuthState
    
    var body: some View {
        
        ZStack {
            if self.authState.authView == .login {
                LoginView()
            } else if self.authState.authView == .signup {
                SignUpView()
            } else {
                Home()
            }
        }
        .onAppear() {
            self.authState.authView = .signup
        }
        
    }
}

struct ContentView_Previews: PreviewProvider {
    static var previews: some View {
        ContentView()
    }
}

struct TabBarView: View {
    var body: some View {
        HStack {
            ForEach(tabItems, id: \.self) { tab in
                Button(action: {}) {
                    VStack {
                        Image(systemName: "house")
                            .frame(width: 26, height: 26)
                            .font(.system(size: 26))
                            .foregroundColor(.pink)
                        
                        Text(tab)
                            .font(.caption)
                            .foregroundColor(.black)
                            .opacity(0)
                    }
                }
                .frame(width: 70, height: 50)
                .background(Color.black)
            }
        }
    }
}

var tabItems = ["Home", "Search", "Favourites", "Settings"]

struct Home: View {
    
    @State var selected = "Home"
    @State var centerX: CGFloat = 0
    
    let curveRadius: CGFloat = 20
    let curveHeight: CGFloat = 30
    let curveWidth: CGFloat = 30
    
    init() {
        UITabBar.appearance().isHidden = true
    }
    
    var body: some View {
        VStack(spacing: 0) {
            TabView(selection: $selected) {
                Color.white
                    .tag(tabItems[0])
                    .ignoresSafeArea(.all, edges: .vertical)
                
                Color.white
                    .tag(tabItems[1])
                    .ignoresSafeArea(.all, edges: .vertical)
                
                Color.white
                    .tag(tabItems[2])
                    .ignoresSafeArea(.all, edges: .vertical)
                
                Color.white
                    .tag(tabItems[3])
                    .ignoresSafeArea(.all, edges: .vertical)
            }
            
            HStack(spacing: 0) {
                ForEach(tabItems, id: \.self) { value in
                    
                    GeometryReader { reader in
                        TabBarButton(selected: $selected, centerX: $centerX, value: value, rect: reader.frame(in: .global))
                            .onAppear(perform: {
                                if value == tabItems.first {
                                    centerX = reader.frame(in: .global).midX
                                }
                            })
                    }
                    .frame(width: 70, height: 50)
                    
                    if value != tabItems.last {
                        Spacer(minLength: 0)
                    }
                }
            }
            .padding(.horizontal, 40)
            .padding(.top, 20)
            .padding(.bottom, 40)
            .background(Color("MainColor").clipShape(AnimatedShape(centerX: centerX, curveRadius: curveRadius, curveHeight: curveHeight, curveWidth: curveWidth)))
//            .shadow(color: Color.black.opacity(0.1), radius: 5, x: 0, y: -5)
            .padding(.top, -(curveHeight + curveRadius))
            
        }
        .ignoresSafeArea(.all, edges: .bottom)
    }
}

struct TabBarButton: View {
    
    @Binding var selected: String
    @Binding var centerX: CGFloat
    
    var value: String
    var rect: CGRect
    
    var body: some View {
        Button(action: {
            withAnimation(.spring()) {
                selected = value
                centerX = rect.midX
            }
        }) {
            VStack {
                Image(systemName: "house")
                    .frame(width: 26, height: 26)
                    .font(.system(size: 26))
                    .foregroundColor(.white)
                
                Text(value)
                    .font(.caption)
                    .foregroundColor(.white)
                    .opacity(selected == value ? 1 : 0)
            }
            .padding(.top)
            .frame(width: 70, height: 50)
            .offset(y: selected == value ? -10 : 15)
        }
    }
}

struct AnimatedShape: Shape {
    
    var centerX: CGFloat
    let curveRadius: CGFloat
    let curveHeight: CGFloat
    let curveWidth: CGFloat
    
    var animatableData: CGFloat {
        get { return centerX }
        set { centerX = newValue }
    }
    
    func path(in rect: CGRect) -> Path {
        
        return Path { path in
            path.move(to: CGPoint(x: 0, y: rect.height))
            path.addLine(to: CGPoint(x: 0, y: (curveHeight + curveRadius)))
            path.addQuadCurve(to: CGPoint(x: curveRadius, y: curveHeight), control: CGPoint(x: 0, y: curveHeight))
            path.addLine(to: CGPoint(x: rect.width - curveRadius, y: curveHeight))
            path.addQuadCurve(to: CGPoint(x: rect.width, y: (curveHeight + curveRadius)), control: CGPoint(x: rect.width, y: curveHeight))
            path.addLine(to: CGPoint(x: rect.width, y: rect.height))
            
            
            path.move(to: CGPoint(x: centerX - curveWidth * 2, y: curveHeight))
            
            let to1 = CGPoint(x: centerX, y: 0)
            let control1 = CGPoint(x: centerX - curveWidth, y: curveHeight)
            let control2 = CGPoint(x: centerX - curveWidth, y: 0)
            
            let to2 = CGPoint(x: centerX + curveWidth * 2, y: curveHeight)
            let control3 = CGPoint(x: centerX + curveWidth, y: 0)
            let control4 = CGPoint(x: centerX + curveWidth, y: curveHeight)
            
            path.addCurve(to: to1, control1: control1, control2: control2)
            path.addCurve(to: to2, control1: control3, control2: control4)
//            path.addQuadCurve(to: CGPoint(x: centerX + 35, y: 15), control: CGPoint(x: centerX, y: -30))
        }
    }
}
