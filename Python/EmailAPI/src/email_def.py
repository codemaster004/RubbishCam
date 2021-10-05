from flask import Flask
from flask import request
from flask import jsonify
import smtplib
from email.message import EmailMessage
import os


email = os.environ.get('EMAIL')
email_password = os.environ.get('EMAIL_PASSWORD')
api_password = os.environ.get('API_PASSWORD')


app = Flask(__name__)


@app.route("/send_mail", methods=['POST'])
def send_email():
    password = request.json['password']
    subject = request.json['subject']
    receiver = request.json['receiver']
    content = request.json['content']
    
    if password != api_password:
        return jsonify({'status': 401, 'title': 'Unauthorized', 'detail': 'Incorrect password'}), 401
    
    try:
        msg = EmailMessage()
        msg['Subject'] = subject
        msg['From'] = email
        msg['To'] = receiver
        msg.set_content(content)
    
        with smtplib.SMTP_SSL('smtp.gmail.com', 465) as smtp:
            smtp.login(email, email_password)
    
            smtp.send_message(msg)
    except Exception as e:
        print('Error in sending email:', e)
        return jsonify({'status': 500, 'title': 'Initial Server Error'}), 500
    
    return '', 200


if __name__ == "__main__":
    app.run(host='0.0.0.0')
    # app.run()
