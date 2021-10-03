import csv
import dataset

# Load class map - these tables map the original TACO classes to your desired class system
# and allow you to discard classes that you don't want to include.
class_map = {}
with open("./taco_config/map_17.csv") as csvfile:
    reader = csv.reader(csvfile)
    class_map = {row[0]: row[1] for row in reader}

# Load full dataset or a subset
TACO_DIR = "../data"
round = None  # Split number: If None, loads full dataset else if int > 0 selects split no 
subset = "train"  # Used only when round !=None, Options: ('train','val','test') to select respective subset
dataset = dataset.Taco()
taco = dataset.load_taco(TACO_DIR, round, subset, class_map=class_map, return_taco=True)

# Must call before using the dataset
dataset.prepare()

# print("Class Count: {}".format(dataset.num_classes))
# for i, info in enumerate(dataset.class_info):
#     print("{:3}. {:50}".format(i, info['name']))

images = [

]
n = 20

for img in taco.dataset['images'][:n]:
    print(img)
    img_data = {
        'folder': 'data/' + img['file_name'].split('/')[0],
        'filename': img['file_name'].split('/')[1],
        'path': 'data/' + img['file_name'],
        'size': {
            'width': img['width'],
            'height': img['height']
        },
        'objects': []
    }
    images.append(img_data)

for ann in taco.dataset['annotations']:
    if ann['image_id'] < n:
        obj_data = {
            'name': dataset.class_info[ann['category_id']]['name'].replace(' ', ''),
            'bndbox': {
                'xmin': ann['bbox'][0],
                'ymin': ann['bbox'][1],
                'xmax': ann['bbox'][0] + ann['bbox'][2],
                'ymax': ann['bbox'][1] + ann['bbox'][3]
            }
        }
        images[ann['image_id']]['objects'].append(obj_data)

print(images)
