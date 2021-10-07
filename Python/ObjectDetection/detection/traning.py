import turicreate as tc

data = tc.SFrame('taco.sframe')
train_data, test_data = data.random_split(0.8)

model = tc.object_detector.create(train_data, max_iterations=2500, batch_size=32)

predictions = model.predict(test_data)
metrics = model.evaluate(test_data)
print(metrics)

model.save('taco.model')
model.export_coreml('GarbageObjectDetector.mlmodel')
