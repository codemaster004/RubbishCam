import grpc
import email_pb2
import email_pb2_grpc


channel = grpc.insecure_channel('0.0.0.0:50051')

stub = email_pb2_grpc.EmailerStub(channel)

email = email_pb2.EmailRequest(
    password='HelloWorld123',
    receiver='filip.dabkowski@gmail.com',
    subject='Test',
    content='Hello Docker!'
)

response = stub.SendEmail(email)

print(response)
