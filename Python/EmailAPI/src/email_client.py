import grpc
import email_pb2
import email_pb2_grpc


channel = grpc.insecure_channel('localhost:50051')

stub = email_pb2_grpc.EmailerStub(channel)

email = email_pb2.EmailRequest(
    password='',
    receiver='',
    subject='Test',
    content='Hello World!'
)

response = stub.SendEmail(email)

print(response)
