from PIL import Image
from io import BytesIO
import requests


content_type = 'image/jpeg'
headers = {'content-type': content_type}

files = {'file': ('test.jpg', open('images/test_image.jpg', 'rb'), content_type)}

requests.post('http://localhost:5000/upload', files=files)
