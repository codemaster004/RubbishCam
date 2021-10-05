from PIL import Image
from io import BytesIO
import requests


content_type = 'image/jpeg'
headers = {'content-type': content_type}

# image = Image.open('test_image.jpg')
# with BytesIO() as output:
#     image.save(output, format="GIF")
#     contents = output.getvalue()

# fd = open('screen.jpg', 'wb')
# fd.write(contents)
# fd.close()
# fin = open('screen.jpg', 'wb')
files = {'file': ('test.jpg', open('test_image.jpg','rb'), content_type)}

requests.post('http://localhost:5000/upload', files=files)
