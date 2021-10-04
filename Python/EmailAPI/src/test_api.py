import requests

data = {
    'password': '',
    'receiver': '',
    'subject': 'Test New',
    'content': 'Hello Flask'
}

r = requests.post('http://0.0.0.0:5000/send_mail', json=data)

print(r.text)
