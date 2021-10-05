from flask import Flask
from flask import request
from flask import jsonify
from werkzeug.utils import secure_filename
import os

app = Flask(__name__)


@app.route('/upload', methods=['POST'])
def upload():
    try:
        file = request.files['file']
        
    except Exception as e:
        print('Error with file:', e)
        return jsonify({'status': 500, 'title': 'Houston we may have a problem.'}), 500
    
    return '', 200


if __name__ == '__main__':
    app.run()
