from PIL import Image
from io import BytesIO
import requests


content_type = 'image/jpeg'
headers = {'content-type': content_type}

files = {'file': ('test.jpg', open('/Users/filip/GitHub/RubbishCam/Python/ObjectDetection/ImageService/src/images/test_image.jpg', 'rb'), content_type)}

r = requests.post('http://0.0.0.0:5000/upload', files=files)

print(r)
# print(r.json())
