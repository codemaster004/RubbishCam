import turicreate as tc

# Create a model
model = tc.load_model('taco.model')

images = [tc.Image(path='../dataset/batch_6/000006.JPG')]

test = tc.SFrame({'image': images})

model.export_coreml('GarbageObjectDetector.mlmodel')

# Save predictions to an SArray
predictions = model.predict(test)
print(predictions)

# Evaluate the model and save the results into a dictionary
# metrics = model.evaluate(test_data)

test['predictions'] = model.predict(test)

test['image_with_predictions'] = \
    tc.object_detector.util.draw_bounding_boxes(test['image'], test['predictions'])
test[['image', 'image_with_predictions']].explore()

# Save the model for later use in Turi Create
# model.save('taco.model')

# Export for use in Core ML
# model.export_coreml('MyCustomObjectDetector.mlmodel')
