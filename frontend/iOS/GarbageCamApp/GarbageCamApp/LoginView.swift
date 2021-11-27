//
//  LoginView.swift
//  GarbageCamApp
//
//  Created by Filip Dabkowski on 26/11/2021.
//

import SwiftUI

struct LoginView: View {
    
    @State var username = ""
    @State var password = ""
    
    var body: some View {
        ZStack {
            
            Color("BgcGray")
                .ignoresSafeArea()
                .onTapGesture {
                    UIApplication.shared.endEditing()
                }
            
            VStack(spacing: 25) {
                VStack {
                    Image("Logo")
                        .resizable()
                        .scaledToFit()
                    .frame(width: 130)
                    
                    Text("Log In")
                        .foregroundColor(Color("MainColor"))
                        .fontWeight(.bold)
                        .font(.system(size: 24))
                }
                
                // Username Input
                VStack(alignment: .leading, spacing: 7) {
                    Text("Username")
                        .foregroundColor(Color("TextSecondary"))
                        .fontWeight(.medium)
                    
                    TextField("Enter Your Username", text: $username, onCommit:  {
                        UIApplication.shared.endEditing()
                    })
                    .padding()
                    .frame(height: 59)
                    .background(Color.white)
                    .cornerRadius(15)
                    .overlay(
                        RoundedRectangle(cornerRadius: 15)
                            .stroke(Color("AccentGrey"), lineWidth: 1)
                    )
                }
                
                // Password Input
                VStack(alignment: .leading, spacing: 7) {
                    Text("Password")
                        .foregroundColor(Color("TextSecondary"))
                        .fontWeight(.medium)
                    
                    SecureField("Create Password", text: $password, onCommit:  {
                        UIApplication.shared.endEditing()
                    })
                        .padding(.horizontal)
                    .frame(height: 59)
                    .background(Color.white)
                    .cornerRadius(15)
                    .overlay(
                        RoundedRectangle(cornerRadius: 15)
                            .stroke(Color("AccentGrey"), lineWidth: 1)
                    )
                    
                    HStack {
                        Spacer()
                        
                        Text("Forgot password?")
                            .foregroundColor(Color("MainColor"))
                    }
                }
                
                Spacer()
                
                VStack {
                    HStack {
                        Text("Don't have an account?")
                            .foregroundColor(Color("TextSecondary"))
                            .fontWeight(.medium)
                        
                        Text("Sign up")
                            .foregroundColor(Color("MainColor"))
                            .fontWeight(.medium)
                    }
                    
                    MainButton(text: "Get Started") {
                        print("Login in")
                        UIApplication.shared.endEditing()
                    }
                }
            }
            .padding(.horizontal, 20)
            .padding(.vertical)
        }
    }
}

struct LoginView_Previews: PreviewProvider {
    static var previews: some View {
        LoginView()
    }
}

extension UIApplication {
    func endEditing() {
        sendAction(#selector(UIResponder.resignFirstResponder), to: nil, from: nil, for: nil)
    }
}

struct MainButton: View {
    
    let text: String
    let action: () -> Void
    
    var body: some View {
        Button(action: {
            self.action()
        }) {
            HStack {
                Spacer()
            
            Text(text)
                .fontWeight(.bold)
                .font(.system(size: 20))
                .frame(height: 60)
                .foregroundColor(Color.white)
            
                
                Spacer()
            }
            .background(Color("MainColor"))
            .cornerRadius(10)
            .shadow(color: Color("MainColor").opacity(0.7), radius: 9, x: 0, y: 4)
        }
        .buttonStyle(PlainButtonStyle())
    }
}
