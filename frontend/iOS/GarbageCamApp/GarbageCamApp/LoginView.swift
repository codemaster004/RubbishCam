//
//  LoginView.swift
//  GarbageCamApp
//
//  Created by Filip Dabkowski on 26/11/2021.
//

import SwiftUI

struct LoginView: View {
    
    @EnvironmentObject var authState: AuthState
    
    @State var email: String = ""
    @State var password: String = ""
    
    var body: some View {
        ZStack {
            
            Color("BgcGray")
                .ignoresSafeArea()
                .onTapGesture {
                    UIApplication.shared.endEditing()
                }
            
            VStack(spacing: 10) {
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
                
                VStack(spacing: 20) {
                    // Email Input
                    AuthTextField(text: $email, hint: "Email")
                    
                    // Password Input
                    AuthSecretField(text: $password)
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
                            .onTapGesture {
                                withAnimation(.spring()) {
                                    self.authState.authView = AuthViews.signup
                                }
                            }
                    }
                    
                    MainButton(text: "Get Started") {
                        print("Login in")
                        
                        UIApplication.shared.endEditing()
                        
                        withAnimation(.spring()) {
                            self.authState.authView = AuthViews.none
                        }
                    }
                }
            }
            .padding(.horizontal, 20)
            .padding(.vertical)
        }
    }
}

struct SignUpView: View {
    
    @EnvironmentObject var authState: AuthState
    
    @State var email: String = ""
    @State var fullname: String = ""
    @State var password: String = ""
    
    var body: some View {
        ZStack {
            
            Color("BgcGray")
                .ignoresSafeArea()
                .onTapGesture {
                    UIApplication.shared.endEditing()
                }
            
            VStack(spacing: 10) {
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
                
                VStack(spacing: 20) {
                    // Full name Input
                    AuthTextField(text: $fullname, hint: "Full name")
                    
                    // Email Input
                    AuthTextField(text: $email, hint: "Email")
                    
                    // Password Input
                    AuthSecretField(text: $password)
                }
                
                Spacer()
                    .frame(minHeight: 0)
                
                VStack {
                    HStack {
                        Text("Already have an account?")
                            .foregroundColor(Color("TextSecondary"))
                            .fontWeight(.medium)
                        
                        Text("Log in")
                            .foregroundColor(Color("MainColor"))
                            .fontWeight(.medium)
                            .onTapGesture {
                                withAnimation(.spring()) {
                                    self.authState.authView = AuthViews.login
                                }
                            }
                    }
                    
                    MainButton(text: "Get Started") {
                        print("Signed up")
                        
                        UIApplication.shared.endEditing()
                        
                        withAnimation(.spring()) {
                            self.authState.authView = AuthViews.none
                        }
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
        Group {
            LoginView()
            SignUpView()
        }
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

struct AuthTextField: View {
    
    @Binding var text: String
    
    let hint: String
    
    var body: some View {
        VStack(alignment: .leading, spacing: 7) {
            Text(hint)
                .foregroundColor(Color("TextSecondary"))
                .fontWeight(.medium)
            
            TextField("Enter Your \(hint)", text: $text, onCommit:  {
                UIApplication.shared.endEditing()
            })
                .padding()
                .frame(height: 50)
                .background(Color.white)
                .cornerRadius(15)
                .overlay(
                    RoundedRectangle(cornerRadius: 15)
                        .stroke(Color("AccentGrey"), lineWidth: 1)
                )
        }
    }
}

struct AuthSecretField: View {
    
    @Binding var text: String
    
    var body: some View {
        VStack(alignment: .leading, spacing: 7) {
            Text("Password")
                .foregroundColor(Color("TextSecondary"))
                .fontWeight(.medium)
            
            SecureField("Create Password", text: $text, onCommit:  {
                UIApplication.shared.endEditing()
            })
                .padding(.horizontal)
                .frame(height: 50)
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
    }
}
