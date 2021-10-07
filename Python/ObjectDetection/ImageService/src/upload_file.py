from flask import Flask
from flask import request
from flask import jsonify
import turicreate as tc
from werkzeug.utils import secure_filename
import os


app = Flask(__name__)

model = tc.load_model('/app/src/models/taco.model')


@app.route('/upload', methods=['POST'])
def upload():
    
    try:
        file = request.files['file']
    
        if file.filename == '':
            print('No selected file')
    
        filename = secure_filename(file.filename)
        file.save(os.path.join('images', filename))
        
        image = [tc.Image(path=f'/app/src/images/{filename}')]
        test = tc.SFrame({'image': image})
        predictions = model.predict(test)
        print(predictions)
        print(predictions[0])
    
        return '', 200
        
    except Exception as e:
        print('Error with file:', e)
        return jsonify({'status': 500, 'title': 'Houston we may have a problem.'}), 500
    

if __name__ == '__main__':
    app.run()
