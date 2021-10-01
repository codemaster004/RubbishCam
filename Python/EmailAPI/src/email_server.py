from concurrent import futures
import logging

import grpc
import email_pb2
import email_pb2_grpc

import email_def


class Emailer(email_pb2_grpc.EmailerServicer):

    def SendEmail(self, request, context):
        response = email_pb2.WasSuccessful()
        response.value = email_def.send_email(request.password, request.receiver, request.subject, request.content)
        return response


def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    
    email_pb2_grpc.add_EmailerServicer_to_server(Emailer(), server)
    
    server.add_insecure_port('[::]:50051')
    server.start()
    
    server.wait_for_termination()


if __name__ == '__main__':
    print('Starting up the server...')
    logging.basicConfig()
    serve()
    