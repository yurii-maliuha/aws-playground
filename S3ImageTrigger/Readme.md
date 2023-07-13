
# S3 trigger to generate image thumbnail
**Prerequisite**
* Create _yma-static-resources_ and _yma-static-resources-thumbnails_ buckets
* Create and deploy _s3-img-trigger_ function
* Create _s3-trigger-tutorial_ IAM policy and execution role for the above function

_s3-trigger-tutorial_ IAM policy definition:
```json
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Effect": "Allow",
            "Action": [
                "logs:PutLogEvents",
                "logs:CreateLogGroup",
                "logs:CreateLogStream"
            ],
            "Resource": "arn:aws:logs:*:*:*"
        },
        {
            "Effect": "Allow",
            "Action": [
                "s3:Get*",
                "s3:List*",
                "s3-object-lambda:Get*",
                "s3-object-lambda:List*"
            ],
            "Resource": [
                "arn:aws:s3:::yma-static-resources",
                "arn:aws:s3:::yma-static-resources/*"
            ]
        },
        {
            "Effect": "Allow",
            "Action": [
                "s3:*",
                "s3-object-lambda:*"
            ],
            "Resource": [
                "arn:aws:s3:::yma-static-resources-thumbnails",
                "arn:aws:s3:::yma-static-resources-thumbnails/*"
            ]
        }
    ]
}
```

 **Flow:**
 * Upload image to _yma-static-resources_ bucket
 * _s3-img-trigger_ function is triigered
 * Image thumbnail is uploaded to _yma-static-resources-thumbnails_
