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
        file.save(os.path.join('/app/src/images', filename))

        image = [tc.Image(path=f'/app/src/images/{filename}')]
        test = tc.SFrame({'image': image})
        predictions = model.predict(test)
        # print(predictions)
        # print(predictions[0])
        
        rubbish_types = {
            'metal': ['Aluminiumfoil', 'Can', 'Metalbottlecap'],
            'plastic': ['Cup', 'Plasticbottle', 'Plasticbottlecap', 'Plasticcontainer', 'Plasticfilm', 'Plasticlid', 'Straw'],
            'paper': ['Carton', 'Paper', 'Wrapper'],
            'glass': ['Glassbottle'],
            'normal': ['Styrofoampiece']
        }
        
        my_pred = []
        for data in predictions:
            new_data = data[0]
            for k, v in rubbish_types.items():
                if new_data['label'] in v:
                    new_data['rubbish_type'] = k
            
            my_pred.append(new_data)

        return jsonify(my_pred), 200

    except Exception as e:
        print('Error with file:', e)
        return jsonify({'status': 500, 'title': 'Houston we may have a problem.', 'message': f'{e}'}), 500
    

if __name__ == '__main__':
    # app.run()
    app.run(host='0.0.0.0')
