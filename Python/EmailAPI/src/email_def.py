import os
import smtplib
from email.message import EmailMessage


email = os.environ.get('EMAIL')
email_password = os.environ.get('EMAIL_PASSWORD')
api_password = os.environ.get('API_PASSWORD')


def send_email(password, receiver, subject, content):
    if password != api_password:
        return False
    
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
        return False
    
    return True
