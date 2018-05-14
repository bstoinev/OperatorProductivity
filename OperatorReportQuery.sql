DECLARE @StartDate DATETIME2(7) = '2010-01-01 00:00:00'
DECLARE @EndDate DATETIME2(7) = NULL
DECLARE @WebSite VARCHAR(64) = NULL
DECLARE @Device VARCHAR(64) = NULL
;
WITH cteConversationsFilter AS
(
	SELECT * 
	FROM [Conversation]
	WHERE ((@StartDate IS NULL) OR (StartDate >= @StartDate))
		AND ((@EndDate IS NULL) OR (EndDate <= @EndDate))
		AND ((@WebSite IS NULL) OR (Website = @WebSite))
),
cteMessagesFilter AS
(
	SELECT m.*
	FROM [Messages] m 
		INNER JOIN cteConversationsFilter c ON c.ConversationID = m.ConversationID
		LEFT JOIN Visitor v ON m.MessageUserID = v.VisitorID
	WHERE (@Device IS NULL) OR (v.Device = @Device)
)
, cteChatFirstMessage AS
(
	SELECT m.ConversationID, MIN(m.MessageID) AS FirstMessageID
	FROM cteMessagesFilter m
	GROUP BY m.ConversationID
)
, cteOperatorProactiveChat AS 
(
	SELECT cfm.ConversationID
	FROM cteChatFirstMessage cfm
		INNER JOIN cteMessagesFilter m ON m.MessageID = cfm.FirstMessageID
	WHERE m.MessageFrom = 'Operator'
)
, cteVisitorProactiveChat AS 
(
	SELECT cfm.ConversationID
	FROM cteChatFirstMessage cfm
		INNER JOIN cteMessagesFilter m ON m.MessageID = cfm.FirstMessageID
	WHERE m.MessageFrom = 'Visitor'
)
, cteLastVisitorMessage AS 
(
	SELECT m.ConversationID, MAX(m.MessageID) AS LastMessageID
	FROM cteMessagesFilter m
	WHERE m.MessageFrom = 'Visitor'
	GROUP BY m.ConversationID
)
, cteLastOperatorMessage AS 
(
	SELECT m.ConversationID, MAX(m.MessageID) AS LastMessageID
	FROM cteMessagesFilter m
	WHERE m.MessageFrom = 'Operator'
	GROUP BY m.ConversationID
)
, cteVisitorFollowedChat AS 
(
	SELECT opc.ConversationID
	FROM cteOperatorProactiveChat opc 
		INNER JOIN cteLastVisitorMessage lvm ON lvm.ConversationID = opc.ConversationID		
)
, cteOperatorFollowedChat AS 
(
	SELECT vpc.ConversationID
	FROM cteVisitorProactiveChat vpc 
		INNER JOIN cteLastOperatorMessage lom ON lom.ConversationID = vpc.ConversationID		
)

, cteChatStatistics AS 
(
	SELECT c.OperatorID, 
		COUNT(cfm.ConversationID) AS ProactiveSent, 
		COUNT(vfc.ConversationID) AS ProactiveAnswered,
		COUNT(vpc.ConversationID) AS ReactiveReceived,
		COUNT(ofc.ConversationID) AS ReactiveAnswered,
		SUM(DATEDIFF(SECOND, StartDate, EndDate)) TotalLength,
		AVG(DATEDIFF(SECOND, StartDate, EndDate)) AS AverageLength
	FROM [cteConversationsFilter] c
		LEFT JOIN cteOperatorProactiveChat cfm ON cfm.ConversationID = c.ConversationID
		LEFT JOIN cteVisitorFollowedChat vfc ON vfc.ConversationID = c.ConversationID
		LEFT JOIN cteVisitorProactiveChat vpc ON vpc.ConversationID = c.ConversationID
		LEFT JOIN cteOperatorFollowedChat ofc ON ofc.ConversationID = c.ConversationID
	GROUP BY c.OperatorID
)

SELECT o.OperatorID, o.[Name], cs.ProactiveSent, cs.ProactiveAnswered, cs.ReactiveReceived, cs.ReactiveAnswered, cs.TotalLength, cs.AverageLength
FROM Operator o	LEFT JOIN cteChatStatistics cs ON cs.OperatorID = o.OperatorID
