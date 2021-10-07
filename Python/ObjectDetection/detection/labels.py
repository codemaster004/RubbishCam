import csv
import dataset


class_map = {}
with open("./taco_config/map_17.csv") as csvfile:
    reader = csv.reader(csvfile)
    class_map = {row[0]: row[1] for row in reader}

TACO_DIR = "../dataset"
round = None
subset = "train"
dataset = dataset.Taco()
taco = dataset.load_taco(TACO_DIR, round, subset, class_map=class_map, return_taco=True)

dataset.prepare()

images = []
wanted_ids = {}

for img in taco.dataset['images'][:]:
    wanted_ids[img['id']] = img['file_name']


for ann in taco.dataset['annotations']:
    if ann['image_id'] in wanted_ids:
        obj_data = {
            'image': wanted_ids[ann['image_id']],
            'id': ann['id'],
            'name': dataset.class_info[ann['category_id']]['name'].replace(' ', ''),
            'xMin': ann['bbox'][0],
            'xMax': ann['bbox'][0] + ann['bbox'][2],
            'yMin': ann['bbox'][1],
            'yMax': ann['bbox'][1] + ann['bbox'][3],
        }
        images.append(obj_data)


with open('../dataset/dataset.csv', 'w', encoding='UTF8') as f:
    writer = csv.writer(f)
    
    for data in images:
        writer.writerow(data.values())
