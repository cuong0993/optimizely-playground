Campaign ID: {{campaign.id}}

Campaign name: {{campaign.name}}

Content ID: {{content.id}}

Content name: {{content.name}}

Site URL: {{zaius.account.site_url}}

Company address: {{zaius.account.company_address}}

Company name: {{zaius.account.company_name}}

{% assign result = curl('https://api.zaius.com/v3/profiles').post.body('{"attributes": {"email":"cuong.nguyendinh@zaius.com","last_shown_product_id": "22222"}}').headers(content-type: 'application/json',x-api-key: 'SRj6VKDPl54lbCZoCdVXmw.3NMncfyTBo3m7sMH13IpyGeP62RyjMmc4JWnUC_RJJY') %}

{{result}}

{% assign result = curl('https://api.zaius.com/v3/graphql').post.body('{"query":"query MyQuery {\n\tcustomer_observation(email: \"{$ customer.email $}\") {\n\t\torder_count\n\t}\n}\n","variables":null}').headers(content-type: 'application/json',x-api-key: 'SRj6VKDPl54lbCZoCdVXmw.3NMncfyTBo3m7sMH13IpyGeP62RyjMmc4JWnUC_RJJY') %}

{{ customer.email }} made {{ result.data.customer_observation.order_count }} order(s)